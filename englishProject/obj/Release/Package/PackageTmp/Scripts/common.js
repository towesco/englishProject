function messageShow(info, key) {
    $.getJSON("/User/GetMessage", { key: key }, function (data) {
        if (data) {
            $(".body").html(info);
            $(".alertMessage button:first").attr("data-key", key);

            $(".alertMessage").removeClass("zoomOutRight").show().addClass("animated zoomInRight");
        }
    });
}

function SuccessMessageShow(result) {
    console.log(result);
    if (result) {
        $("#successMessage").show().removeClass("animated bounceOut").addClass("animated bounceIn").one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
            $(this).removeClass("animated bounceIn");
        });

        var timeout = setTimeout(function () {
            $("#successMessage").addClass("animated bounceOut").one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
                $(this).hide();

                clearTimeout(timeout);
            });
        }, 2000);
    } else {
        $("#errorMessage").show().removeClass("animated bounceOut").addClass("animated bounceIn").one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
            $(this).removeClass("animated bounceIn");
        });

        var timeout2 = setTimeout(function () {
            $("#errorMessage").addClass("animated bounceOut").one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
                $(this).hide();

                clearTimeout(timeout2);
            });
        }, 2000);
    }
}

function WarningMessageShow(data) {
    $("#warningMessage strong").html(data);

    $("#warningMessage").show().removeClass("animated bounceOut").addClass("animated bounceIn").one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
        $(this).removeClass("animated bounceIn");
    });

    var timeout = setTimeout(function () {
        $("#warningMessage").addClass("animated bounceOut").one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
            $(this).hide();

            clearTimeout(timeout);
        });
    }, 2000);
}

function GetErrorMessage() {
    return "Hata meydana geldi. Lütfen daha sonra tekrar deneyiniz.";
}

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
    $('[data-toggle="popover"]').popover();
    $(".carousel").carousel({
        interval: false
    });

    ///////////////user/index sayfasındaki box menu///////////////////////////
    $(".boxMenuWrapper li:first").addClass("current");
    $('#box-carausel').on('slid.bs.carousel', function (a) {
        var index = $('#box-carausel .active').index('#box-carausel .item');

        $(".boxMenuWrapper li").removeClass("current");

        $(".boxMenuWrapper li:eq('" + index + "')").addClass("current");
    });

    ///////////////user/index sayfasındaki box menu///////////////////////////

    ///////////////home/index sayfasındaki box menu///////////////////////////
    $('#carousel-learn').on('slid.bs.carousel', function (a) {
        var index = $('#carousel-learn .carousel-inner div.active').index();

        if (index == 4) {
            $("#learnModal .modal-footer a").hide();
            $("#learnModal .modal-footer button").show();
        } else {
            $("#learnModal .modal-footer a").show();
            $("#learnModal .modal-footer button").hide();
        }
    });
    $("#learnModalBtn").click(function () {
        $("#learnModal li,#learnModal div").removeClass("active");
        $("#learnModal ol li:first").addClass("active");
        $("#learnModal .carousel-inner div:first").addClass("active");
        $("#learnModal .modal-footer a").show();
        $("#learnModal .modal-footer button").hide();
    });
    $(".modal-footer button").click(function () {
        $("#learnModal").modal("hide");
        $(".btn-login").trigger("click");
    });
    ///////////////home/index sayfasındaki box menu///////////////////////////

    $(".alertMessage button:first").click(function () {
        var key = $(this).attr("data-key");

        $(".alertMessage").addClass("animated zoomOutRight");

        $.getJSON("/User/GetMessageHide", { key: key }, function () {
        });
    });

    $(".subLevelSelect,#quizBtn").click(function (e) {
        e.preventDefault();

        var locked = $(this).attr("data-locked");
        var front = $(this).attr("data-front");
        if (locked == "False") {
            $("#commonModal .modal-body").html("<h2 class='text-center'><i class='fa fa-spinner fa-pulse'></i> yükleniyor....</h2>");
            $("#commonModal").modal();
            var levelId = $(this).attr("data-levelId");

            var jsonData = { levelId: levelId };

            $.ajax("/User/LevelExamStartAjax", {
                type: "GET",
                data: jsonData,
                contentType: "html",
                success: function (data) {
                    $("#commonModal .modal-body").html(data);
                }
            });
        } else {
            if (front == 1) {
                $(this).attr("data-front", "0");
                $(this).addClass("subLevelSelectReverse");

                $(this).children(".front").fadeOut();
                $(this).children(".back").fadeIn();
            } else {
                $(this).attr("data-front", "1");
                $(this).removeClass("subLevelSelectReverse");

                $(this).children(".front").fadeIn();
                $(this).children(".back").fadeOut();
            }
        }
    });
})