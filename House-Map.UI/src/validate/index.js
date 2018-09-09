
const isNumRe = /^[0-9]+.?[0-9]*$/;
export default {
  isNum(rule, value, callback) {
    if(value){
      if(!isNumRe.test(value)) {
        callback(new Error())
      }else {
        callback()
      }
    }
    callback()
  },
  isInteger(rule, value, callback) {
    if(value && !isNumRe.test(value) || !Number.isInteger(+value)) {
      callback(new Error())
    }else {
      callback()
    }
  }
}