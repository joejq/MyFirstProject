/*获标准格式的日期*/
function getMyDate(date) {
    var tdate = date.replace(/[a-zA-z]/g, " ");
    var str = new Array();
    str = tdate.split(" ");
    return str[0];
}
/*获取指定长度的标题*/
function getMyTitle(title, len) {
    if (title.length <= len) return title;
    else return title.substring(0, len) + "……";
}
/*验证手机号*/
function validateNum(value) {
    var telReg = /^[0-9]{11}$/;
    if (!telReg.test(value)) return false;
    return true;
}
/*验证是不是数字字符串*/
function isNumberString(vlue) {
    var num = parseInt(value);
    var numStr = num + "";
    if (numStr == value) return true;
    return false;
}
/*是否为空*/
function isNullorEmpty(value) {
    if (value == null || value == "") return true;
    return false;
}