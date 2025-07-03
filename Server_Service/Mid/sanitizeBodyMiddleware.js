// sanitizeBodyMiddleware.js

const getRawBody = require('raw-body');
const typer = require('media-typer');

/**
 * Middleware này sẽ:
 * - Đọc body dạng raw
 * - Loại bỏ các control character không hợp lệ (ASCII 0-31 trừ \t, \n, \r)
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

            // Loại bỏ control characters không mong muốn:
            // Thay thế: ASCII từ 0x00 đến 0x08, 0x0B, 0x0C, 0x0E đến 0x1F sang dạng {tên ký tự}
            // Thay thế GS (Group Separator) 0x1D sang {GS} để tránh lỗi parse JSON
            const cleaned = raw.replace(/\u001D/g, '<GS>');

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
