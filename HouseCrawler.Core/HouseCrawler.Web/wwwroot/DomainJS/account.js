$(document).ready(function () { // DOM的onload事件处理函数  
  $('#login_submit').bind('click', function (e) {
    e.preventDefault()
    login()
    e.stopPropagation()
  })

  $('#qq_login').bind('click', function (e) {
    e.preventDefault()
    qq_login()
    e.stopPropagation()
  })
})

function login () {
  var email = $('#aw-login-user-name').val()
  if (email == '') {
    alert('登陆邮箱你都不给我？！！！')
    return
  }

  var password = $('#aw-login-user-password').val()
  if (password == '') {
    alert('莫有密码怎么登陆呀？')
    return
  }

  var userInfo =
  {
    UserEmail: email,
    Password: password
  }

  $.ajax({
    type: 'post',
    url: loginUrl,
    data: { userName: email, password: password },
    success: function (rsp) {
      if (rsp.success) {
        // alert(rsp.SuccessMessage)
        window.location.href = '../'
      } else {
        alert(rsp.error)
      }
    }
  })
}

function qq_login () {
  // 以下为按钮点击事件的逻辑。注意这里要重新打开窗口
  // 否则后面跳转到QQ登录，授权页面时会直接缩小当前浏览器的窗口，而不是打开新窗口
  window.location.href = qqLoginURL;
  //var A = window.open(qqLoginURL, 'TencentLogin', 'width=450,height=320,menubar=0,scrollbars=1,resizable = 1, status = 1, titlebar = 0, toolbar = 0, location = 1')
}
