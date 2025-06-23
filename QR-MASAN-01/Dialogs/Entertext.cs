using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;

namespace Diaglogs
{
    public partial class Entertext : Form
    {
        public event EventHandler EnterClicked;
        public string TextValue { get; set; }
        private bool isShiftEnabled = true; // Trạng thái Shift
        private bool isSymbolEnabled = false; // Trạng thái Symbol
        public string TileText { get; set; } = "Nhập văn bản";

        public bool IsPassword { get; set; } = false; // Biến để xác định có phải nhập mật khẩu hay không
        public Entertext()
        {
            InitializeComponent();

            
        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            //// Gửi phím về MainForm
            TextValue = textPadTextBox.Text;
            EnterClicked?.Invoke(this, EventArgs.Empty);
            // Đóng form với kết quả OK
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Shift_Click(object sender, EventArgs e)
        {
            // Đổi trạng thái Shift
            isShiftEnabled = !isShiftEnabled;
            Shift.FillColor = isShiftEnabled ? Color.FromArgb(255, 128, 0) : Color.FromArgb(0, 122, 204);
            UpdateKeyboardKeys();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            int cursorPosition = textPadTextBox.SelectionStart; // Vị trí con trỏ
            if (sender is UIButton button)
            {
                string key = button.Text;
                // Thêm khoảng trắng tại vị trí con trỏ
                textPadTextBox.Text = textPadTextBox.Text.Insert(cursorPosition, key);
                textPadTextBox.SelectionStart = cursorPosition + 1; // Di chuyển con trỏ
            }
        }

        private void UpdateKeyboardKeys()
        {
            A.Text = isShiftEnabled ? "A" : "a";
            B.Text = isShiftEnabled ? "B" : "b";
            C.Text = isShiftEnabled ? "C" : "c";
            D.Text = isShiftEnabled ? "D" : "d";
            E.Text = isShiftEnabled ? "E" : "e";
            F.Text = isShiftEnabled ? "F" : "f";
            G.Text = isShiftEnabled ? "G" : "g";
            H.Text = isShiftEnabled ? "H" : "h";
            I.Text = isShiftEnabled ? "I" : "i";
            J.Text = isShiftEnabled ? "J" : "j";
            K.Text = isShiftEnabled ? "K" : "k";
            L.Text = isShiftEnabled ? "L" : "l";
            M.Text = isShiftEnabled ? "M" : "m";
            N.Text = isShiftEnabled ? "N" : "n";
            O.Text = isShiftEnabled ? "O" : "o";
            P.Text = isShiftEnabled ? "P" : "p";
            Q.Text = isShiftEnabled ? "Q" : "q";
            R.Text = isShiftEnabled ? "R" : "r";
            S.Text = isShiftEnabled ? "S" : "s";
            T.Text = isShiftEnabled ? "T" : "t";
            U.Text = isShiftEnabled ? "U" : "u";
            V.Text = isShiftEnabled ? "V" : "v";
            W.Text = isShiftEnabled ? "W" : "w";
            X.Text = isShiftEnabled ? "X" : "x";
            Y.Text = isShiftEnabled ? "Y" : "y";
            Z.Text = isShiftEnabled ? "Z" : "z";

        }

        private void S123_Click(object sender, EventArgs e)
        {
            isSymbolEnabled = !isSymbolEnabled;

            Q.Text = isSymbolEnabled ? "1" : "Q";
            W.Text = isSymbolEnabled ? "2" : "W";
            E.Text = isSymbolEnabled ? "3" : "E";
            R.Text = isSymbolEnabled ? "4" : "R";
            T.Text = isSymbolEnabled ? "5" : "T";
            Y.Text = isSymbolEnabled ? "6" : "Y";
            U.Text = isSymbolEnabled ? "7" : "U";
            I.Text = isSymbolEnabled ? "8" : "I";
            O.Text = isSymbolEnabled ? "9" : "O";
            P.Text = isSymbolEnabled ? "0" : "P";
            A.Text = isSymbolEnabled ? "@" : "A";
            S.Text = isSymbolEnabled ? "#" : "S";
            D.Text = isSymbolEnabled ? "$" : "D";
            F.Text = isSymbolEnabled ? "%" : "F";
            G.Text = isSymbolEnabled ? "&" : "G";
            H.Text = isSymbolEnabled ? "*" : "H";
            J.Text = isSymbolEnabled ? "(" : "J";
            K.Text = isSymbolEnabled ? ")" : "K";
            L.Text = isSymbolEnabled ? "-" : "L";
            Z.Text = isSymbolEnabled ? "+" : "Z";
            X.Text = isSymbolEnabled ? "=" : "X";
            C.Text = isSymbolEnabled ? "/" : "C";
            V.Text = isSymbolEnabled ? "\\" : "V";
            B.Text = isSymbolEnabled ? "|" : "B";
            N.Text = isSymbolEnabled ? "~" : "N";
            M.Text = isSymbolEnabled ? "<" : "M";


            Shift.Enabled = isSymbolEnabled ? false : true;
            isShiftEnabled = true;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textPadTextBox.Text = "";
        }

        private void Space_Click(object sender, EventArgs e)
        {
            int cursorPosition = textPadTextBox.SelectionStart; // Vị trí con trỏ
            // Thêm khoảng trắng tại vị trí con trỏ
            textPadTextBox.Text = textPadTextBox.Text.Insert(cursorPosition, " ");
            textPadTextBox.SelectionStart = cursorPosition + 1; // Di chuyển con trỏ
        }

        private void uiSymbolButton2_Click(object sender, EventArgs e)
        {
            int cursorPosition = textPadTextBox.SelectionStart; // Vị trí con trỏ
            if (cursorPosition > 0)
            {
                // Xóa ký tự trước con trỏ
                textPadTextBox.Text = textPadTextBox.Text.Remove(cursorPosition - 1, 1);
                textPadTextBox.SelectionStart = cursorPosition - 1; // Cập nhật lại vị trí con trỏ
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Entertext_Load(object sender, EventArgs e)
        {
            textPadTextBox.Text = TextValue;

            // Thiết lập tiêu đề của form
            uiTitlePanel1.Text = TileText;

            // Thiết lập chế độ nhập liệu
            if (IsPassword)
            {
                textPadTextBox.PasswordChar = '*'; // Hiển thị mật khẩu
                btnEyePass.Visible = true; // Hiển thị nút mắt để hiển thị/ẩn mật khẩu
            }
            else
            {
                textPadTextBox.PasswordChar = '\0'; // Hiển thị ký tự thực
                btnEyePass.Visible = false; // Ẩn nút mắt
            }
        }

        private void btnEyePass_Click(object sender, EventArgs e)
        {
            if(textPadTextBox.PasswordChar == '*')
            {
                // Hiển thị mật khẩu
                textPadTextBox.PasswordChar = '\0'; // Hiển thị ký tự thực
                // Cập nhật biểu tượng nút
                btnEyePass.Symbol = 361550; // Biểu tượng mắt mở
            }
            else
            {
                // Ẩn mật khẩu
                textPadTextBox.PasswordChar = '*'; // Hiển thị mật khẩu
                // Cập nhật biểu tượng nút
                btnEyePass.Symbol = 361552; // Biểu tượng mắt đóng
            }
        }
    }
}
