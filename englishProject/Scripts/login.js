var SignInViewModel = function () {
    self.UserSignInVM = {
        email: ko.observable(),
        password: ko.observable(),
        meRemember: ko.observable()
    };

    self.showlogin = ko.observable(true);
    self.changes = function () {
        console.log(self.showlogin());

        if (self.showlogin()) {
            self.showlogin(false);
        } else {
            self.showlogin(true);
        }
    };
    self.loading = ko.observable(false);
    self.noUser = ko.observable(false);

    self.loginSocialForm = function () {
        self.loading(true);
        return true;
    }

    self.validate = function (form) {
        if ($(form).valid()) {
            self.loading(true);
            var jsonData = ko.toJSON(self.UserSignInVM);
            if ($(form).validate()) {
                $.ajax("/api/ajax/SignIn", {
                    type: "POST",
                    data: jsonData,
                    contentType: "application/json",
                    success: function (data) {
                        if (data) {
                            location.href = "/User/Index";
                        } else {
                            self.loading(false);
                            self.noUser(true);
                        }
                    }
                });
            }
        }

        return false;
    }
}

var SignUpViewModel = function () {
    self.UserSignUpVM = {
        email: ko.observable(),
        password: ko.observable(),
        confirmPassword: ko.observable()
    };

    self.signUpError = ko.observable(false);
    self.signupLoading = ko.observable(false);

    self.validateSignUp = function (form) {
        if ($(form).valid()) {
            self.signupLoading(true);
            var jsonData = ko.toJSON(self.UserSignUpVM);
            if ($(form).validate()) {
                $.ajax("/api/ajax/SignUp", {
                    type: "POST",
                    data: jsonData,
                    contentType: "application/json",
                    success: function (data) {
                        if (data == 0) {
                            $("#signUpErrorText").html("Bir hata meydana geldi. Lütfen daha sonra tekrar deneyiniz.");
                            self.signupLoading(false);
                            self.signUpError(true);
                        }
                        else if (data == 1) {
                            location.href = "/User/Index";
                        } else if (data == 2) {
                            $("#signUpErrorText").html("Email adresi kayıtlı.");
                            self.signupLoading(false);
                            self.signUpError(true);
                        }
                    }
                });
            }
        }

        return false;
    }
}

$(document).ready(function () {
    $("#loginWrapper").click(function (e) {
        e.stopPropagation();
    });

    var signInWM = new SignInViewModel();
    var signUpWM = new SignUpViewModel();

    ko.applyBindings(signInWM, document.getElementById("signInWrapper"));
    ko.applyBindings(signUpWM, document.getElementById("signUpWrapper"));
})