// sanitizeBodyMiddleware.js

const getRawBody = require('raw-body');
const typer = require('media-typer');

/**
 * Middleware này sẽ:
 * - Đọc body dạng raw
 * - Loại bỏ các control character không hợp lệ (ASCII 0-31 trừ \t, \n, \r)
 * - Thay thế 0x1D (GS) thành <GS>
 * - Parse JSON thủ công
 * - Gán vào req.body để tránh lỗi crash do JSON invalid
 */
module.exports = async function sanitizeBodyMiddleware(req, res, next) {
    if (
        req.headers['content-type'] &&
        req.headers['content-type'].includes('application/json')
    ) {
        try {
            const raw = await getRawBody(req, {
                length: req.headers['content-length'],
                limit: '50mb',
                encoding:
                    typer.parse(req.headers['content-type']).parameters.charset ||
                    'utf-8',
            });

            // Thay thế các control character không mong muốn thành <CTRL_xx>
            const cleaned = raw
                // Thay GS (0x1D) trước để tách barcode
                .replace(/\u001D/g, '<GS>')
                // Thay toàn bộ ASCII 0-31 trừ \t(0x09), \n(0x0A), \r(0x0D)
                .replace(/[\x00-\x08\x0B\x0C\x0E-\x1C\x1E\x1F]/g, (c) => {
                    const code = c.charCodeAt(0).toString(16).padStart(2, '0').toUpperCase();
                    return `<CTRL_${code}>`;
                });

            // Parse JSON
            req.body = JSON.parse(cleaned);

            next();
        } catch (err) {
            console.error('❌ Lỗi parse JSON hoặc control character:', err);
            res.status(400).json({
                message:
                    'Payload JSON không hợp lệ hoặc chứa ký tự không hợp lệ, vui lòng kiểm tra lại dữ liệu gửi lên.',
                detail: err.message,
            });
        }
    } else {
        next();
    }
};
