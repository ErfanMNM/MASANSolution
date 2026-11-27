"""Execute SQL extracted verbatim from owned production methods against :memory: only."""
import json
import re
import sqlite3
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]


def method_section(relative, signature):
    source = (ROOT / relative).read_text(encoding="utf-8-sig")
    start = source.index(signature)
    end = re.search(r"\n        (?:public |private |#endregion)", source[start + len(signature):])
    return source[start:start + len(signature) + end.start()] if end else source[start:]


def query(relative, signature):
    section = method_section(relative, signature)
    expression = re.search(r"string query = (.*?);[ \t]*\r?\n", section, re.S).group(1)
    if expression.startswith('@"'):
        return expression[2:-1].replace('""', '"')
    parts = re.findall(r'"(?:\\.|[^"\\])*"', expression)
    return "".join(json.loads(part) for part in parts)


class SqlRegression(unittest.TestCase):
    def test_legacy_activation_updates(self):
        for name in ("Main", "C1", "C2"):
            with self.subTest(camera=name), sqlite3.connect(":memory:") as db:
                db.execute("CREATE TABLE QRContent(ProductID INTEGER PRIMARY KEY, Active INTEGER, TimeStampActive TEXT, TimeUnixActive INTEGER, TimeStampPrinted TEXT, TimeUnixPrinted INTEGER)")
                db.execute("INSERT INTO QRContent(ProductID,Active) VALUES(7,0)")
                sql = query("QR-MASAN-01/Views/F1Dashboard.cs", "public void Update_Active_Status_" + name + "(")
                db.execute(sql, dict(RowId=7, TimeStampActive="active", TimeUnixActive=1795730000123, TimestampPrinted="printed", TimeUnixPrinted=1795730000))
                self.assertEqual(db.execute("SELECT Active,TimeUnixActive,TimeUnixPrinted FROM QRContent").fetchone(), (1, 1795730000123, 1795730000))

    def test_failed_activation_remains_failed(self):
        with sqlite3.connect(":memory:") as db:
            db.execute("CREATE TABLE UniqueCodes(Status INTEGER, ActivateDate TEXT, ProductionDate TEXT, ActivateUser TEXT)")
            db.execute("INSERT INTO UniqueCodes(Status) VALUES(0)")
            sql = query("QR-MASAN-01/Views/FDashV2.cs", "public void Update_Active_Status(")
            for status in (-1, 1):
                db.execute(sql, dict(status=status, RowId=1, activateDate="fixture", productionDate="fixture", UserName="fixture"))
                self.assertEqual(db.execute("SELECT Status FROM UniqueCodes").fetchone()[0], status)

    def test_production_count_matches_schema(self):
        with sqlite3.connect(":memory:") as db:
            db.execute("CREATE TABLE ProductionLogs(ID INTEGER PRIMARY KEY, TimeUnix INTEGER)")
            db.execute("INSERT INTO ProductionLogs(TimeUnix) VALUES(1)")
            sql = query("QR-MASAN-01/Mid/Audittrail.cs", "public static int Get_Log_Count()")
            self.assertEqual(db.execute(sql).fetchone()[0], 1)

    def test_scanned_quote_is_a_value(self):
        with sqlite3.connect(":memory:") as db:
            db.execute("CREATE TABLE QRContent(ProductQR TEXT)")
            db.executemany("INSERT INTO QRContent VALUES(?)", [("good",), ("x' OR 1=1 --",)])
            sql = query("QR-MASAN-01/Views/ScanQR.cs", "public (bool Issucces, string message, ProductInfo PData) Get_QR_From_SQLite(")
            self.assertEqual(db.execute(sql, {"Code": "%x' OR 1=1 --%"}).fetchall(), [("x' OR 1=1 --",)])


if __name__ == "__main__":
    unittest.main(verbosity=2)
