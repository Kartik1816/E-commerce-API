let pass=true;
function showPassword() {
  console.log('showPassword');
    var x = document.getElementById("password");
    if (pass) {
        x.type = "text";
        document.getElementById('showpassword').classList.add('d-none');
        document.getElementById('hidepassword').classList.remove('d-none');
        pass=false;

    } else {
        x.type = "password";
        document.getElementById('showpassword').classList.remove('d-none');
        document.getElementById('hidepassword').classList.add('d-none');
        pass=true;
    }
}


var isEmailValid = true;
var isPasswordValid = true;

function validateEmail () {
  var email = $('#email').val();
  if (email.length <= 0) {
    isEmailValid = false;
    $('#email-error').text('Email is required');
  } else if (!email.match(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/)) {
    isEmailValid = false;
    $('#email-error').text('Email is invalid');
  } else {
    $('#email-error').text('');
    isEmailValid = true;
  }
}

function validatePassword () {
  var password = $('#password').val();
    if (password.length <= 0) {
      isPasswordValid = false;
      $('#password-error').text('Password is required');
    } else if (!password.match(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$#!%*?&])[A-Za-z\d@$!%*?&]{8,}$/)) {
      isPasswordValid = false;
      $('#password-error').text('Password must contain at least one numeric digit, one uppercase and one lowercase letter, and at least 6 or more characters');   
    } else {
      $('#password-error').text('');
      isPasswordValid = true;
    }
}

$('#email').on('input', function () {
    $('#login-error').text(''); 
    validateEmail();
    localStorage.setItem('email',$('#email').val());
  });
  
  $('#password').on('input', function () {
    validatePassword();
    $('#login-error').text('');
  });


  $('#login-form').submit(function (e) {

    var rememberMe=$('#rememberMe').is(':checked')

    validateEmail();
    validatePassword();
    e.preventDefault();
    if (!isEmailValid || !isPasswordValid) {
      return;
    }
    var form = $(this)[0];
    if(!$(this).valid())
    {
        return;
    }
    var formData = new FormData(form);
    
  $.ajax({
    url: '/Auth/Login',
    type: 'POST',
    data: formData,
    processData : false,
    contentType : false,
    success: function (data) {
        if (data.success)
        {
            toastr.success(data.message)
            console.log(data.token);
            document.cookie = `token=${data.token};path=/;Samesite=Lax`;
            window.location.href='/Home';
            if(rememberMe){
              var date=new Date();
              date.setTime(date.getTime()+(7*24*60*60*1000))
              document.cookie=`email=${email};expires=${date.toUTCString()}`
              document.cookie = `token=${data.token};max-age=${60*60*24*7};path=/;Samesite=Lax`;
    
            }
        }
        else 
        {
            toastr.error(data.message);
        }
    },
    error: function (err) 
    {
      toastr.error('An error occurred while processing your request');
    }
  });

});

$(document).ready(function(){
  var email = document.cookie.split(';').find(cookie => cookie.includes('email'));
    if (email)
    {
      window.location.href = '/Home';
    }
});
