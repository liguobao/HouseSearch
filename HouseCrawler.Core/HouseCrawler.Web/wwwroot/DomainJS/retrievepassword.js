
$(document).ready(function () {// DOM的onload事件处理函数  
    $('#btnSend').bind('click', function (e) {
        e.preventDefault();
        SendEmail();
        e.stopPropagation();
    });
})

function SendEmail()
{
   
    var email = $("#UserEmail").val();
    $.ajax(
         {
             type: "post",
             url: checkUserEmailURL,
             data: { email: email },
             success:
                 function (rsp) {
                     if (rsp.IsSuccess) {
                         $("#btnSend").text("用户不存在！");
                         return;
                     }else
                     {
                         Send(email);
                     }
                 }
         });
}

function Send(email)
{
    $.ajax(
       {
           type: "post",
           url: sendEmailURL,
           data: { emailAccount: email },
           success:
               function (result) {
                   if (result.IsSuccess) {
                       $("#btnSend").text("发送成功");
                       return;
                   } else {

                       $("#btnSend").text(result.ErrorMessage);
                   }
               }
       });
}