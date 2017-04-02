/**
 * Created by yhy13 on 2017/3/2.
 */
function validateNum(value) {
    var telReg = /^[0-9]{11}$/;
    if (!telReg.test(value)) return false;
    return true;
}
function isNumberString(vlue) {
    var num = parseInt(value);
    var numStr = num + "";
    if (numStr == value) return true;
    return false;
}
function isNullorEmpty(value) {
    if (value == null || value == "") return true;
    return false;
}