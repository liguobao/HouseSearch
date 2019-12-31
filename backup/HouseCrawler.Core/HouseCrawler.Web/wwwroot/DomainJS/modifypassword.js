$(document).ready(function () {// DOM的onload事件处理函数  
    $('#btnRetrievePassword').bind('click', function (e) {
        e.preventDefault();
        var newPassword = $('#NewPassword').val();

        var confirmPassword = $('#ConfirmPassword').val();
        if (newPassword == "")
        {
            $("#lblMessage").text("新密码不能为空。");
           
            return;
        }
        if (newPassword.length < 6)
        {
            $("#lblMessage").text("密码长度不能少于6位。");
            return;
        }

        if (newPassword != confirmPassword)
        {
            $("#lblMessage").text("两次输入的密码不一致，请重新输入。");
            return;
        }

        var token = GetQueryString("token");

        $.ajax(
        {
          type: "post",
          url: retrievePasswordURL,
          data: { token: token, password: newPassword },
          success:
              function (result) {
                  if (result.success) {
                      alert("重置成功!");
                      window.location.href = '../Account';
                  } else {
                      $("#lblMessage").text(result.error);
                      return;
                  }
              }
        });
        e.stopPropagation();
    });
})

function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return (r[2]); return null;
}
