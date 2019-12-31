
$(document).ready(function () {// DOM的onload事件处理函数  
    $('#btnRegister').bind('click', function (e) {
        e.preventDefault();
        RegisterUser();
        e.stopPropagation();
    });

    $('.aw-agreement-btn').click(function () {
        if ($('.aw-register-agreement').is(':visible')) {
            $('.aw-register-agreement').hide();
        }
        else {
            $('.aw-register-agreement').show();
        }
    });
    $('.more-information-btn').click(function () {
        $('.more-information').fadeIn();
        $(this).parent().hide();
    });

  
    $('#agreement_chk').click( function (e) {
        if(!$(this).prop("checked")){
            $('#btnRegister').attr("disabled",true);

        }else{
            $('#btnRegister').attr("disabled",false);
        }
    });


    verify_register_form('#register_form');
});

/* 注册页面验证 */
function verify_register_form(element) {
    $(element).find('[type=text], [type=password]').on({
        focus: function () {
            if (typeof $(this).attr('tips') != 'undefined' && $(this).attr('tips') != '') {
                $(this).parent().append('<span class="aw-reg-tips">' + $(this).attr('tips') + '</span>');
            }
        },
        blur: function () {
            if ($(this).attr('tips') != '') {
                switch ($(this).attr('name')) {
                    case 'user_name':
                        var _this = $(this);
                        $(this).parent().find('.aw-reg-tips').detach();
                        if ($(this).val().length >= 0 && $(this).val().length < 2) {
                            $(this).parent().find('.aw-reg-tips').detach();
                            $(this).parent().append('<span class="aw-reg-tips aw-reg-err"><i class="aw-icon i-err"></i>' + $(this).attr('errortips') + '</span>');
                            return;
                        }
                        if ($(this).val().length > 17) {
                            $(this).parent().find('.aw-reg-tips').detach();
                            $(this).parent().append('<span class="aw-reg-tips aw-reg-err"><i class="aw-icon i-err"></i>' + $(this).attr('errortips') + '</span>');
                            return;
                        }
                        return;

                    case 'email':
                        var _this = $(this);
                        $(this).parent().find('.aw-reg-tips').detach();
                        var emailreg = /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/;
                        if (!emailreg.test($(this).val())) {
                            $(this).parent().find('.aw-reg-tips').detach();
                            $(this).parent().append('<span class="aw-reg-tips aw-reg-err"><i class="aw-icon i-err"></i>' + $(this).attr('errortips') + '</span>');
                            return;
                        }
                        else {
                            var email = $(this).val();
                            $.ajax(
                                 {
                                     type: "post",
                                     url: "./CheckUserEmail",
                                     data: { email},
                                     success:
                                         function (rsp) {
                                             if (!rsp.success) {
                                                 _this.parent().find('.aw-reg-tips').detach();
                                                 _this.parent().append('<span class="aw-reg-tips aw-reg-err"><i class="aw-icon i-err"></i>' + rsp.ErrorMessage + '</span>');
                                             }
                                             else {
                                                 _this.parent().find('.aw-reg-tips').detach();
                                                 _this.parent().append('<span class="aw-reg-tips aw-reg-right"><i class="aw-icon i-followed"></i></span>');
                                             }
                                         }
                                 });
                        }
                        return;

                    case 'password':
                        $(this).parent().find('.aw-reg-tips').detach();
                        if ($(this).val().length >= 0 && $(this).val().length < 6) {
                            $(this).parent().find('.aw-reg-tips').detach();
                            $(this).parent().append('<span class="aw-reg-tips aw-reg-err"><i class="aw-icon i-err"></i>' + $(this).attr('errortips') + '</span>');
                            return;
                        }
                        if ($(this).val().length > 17) {
                            $(this).parent().find('.aw-reg-tips').detach();
                            $(this).parent().append('<span class="aw-reg-tips aw-reg-err"><i class="aw-icon i-err"></i>' + $(this).attr('errortips') + '</span>');
                            return;
                        }
                        else {
                            $(this).parent().find('.aw-reg-tips').detach();
                            $(this).parent().append('<span class="aw-reg-tips aw-reg-right"><i class="aw-icon i-followed"></i></span>');
                        }
                        return;

                }
            }

        }
    });
}





function RegisterUser() {
    var  email = $("#UserEmail").val();
    var  password= $("#UserPassword").val();
    var username = $("#UserName").val();
    var emailreg = /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/;
    if (emailreg.test(email)) {
        $.ajax(
                {
                    type: "post",
                    url: "./CheckUserEmail",
                    data: { email},
                    success:
                        function (result) {
                            if (password != "" && username != "" && result.success)
                            {
                                AddUser();
                            }
                        }
                });
}
   
}

function AddUser() {
    var uiUserInfo =
    {
        UserEmail: $("#UserEmail").val(),
        Password: $("#UserPassword").val(),
        UserName: $("#UserName").val(),
        Phone: $("#UserPhone").val(),
        QQ: $("#UserQQ").val(),
        UserInfoComment: $("#welcome_signature").val(),
    };

    $.ajax({
        type: "post",
        url: "./RegisterUser",
        data: { userName: $("#UserName").val(), userEmail: $("#UserEmail").val(), password: $("#UserPassword").val() },
        success:
            function (rsp) {
                if (rsp.success) {
                    alert(rsp.message);
                    window.location.href = '../Account'; // 跳转到B目录
                }
                else
                    alert(rsp.error);
            }
    });
}