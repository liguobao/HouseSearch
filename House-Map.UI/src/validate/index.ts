
const isNumRe = /^[0-9]+.?[0-9]*$/;
export default {
  isNum(rule:any, value:any, callback:Function) {
    if(value){
      if(!isNumRe.test(value)) {
        callback(new Error())
      }else {
        callback()
      }
    }
    callback()
  },
  isInteger(rule:any, value:any, callback:Function) {
    if(value && !isNumRe.test(value) || !Number.isInteger(+value)) {
      callback(new Error())
    }else {
      callback()
    }
  }
}