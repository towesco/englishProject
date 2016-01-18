$(document).ready(function () {
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
    $("button.close").click(function () {
        var key = $(this).attr("data-key");
        $.getJSON("/User/MessageHide", { key: key }, function () {
            console.log("oldu");
        });
    });

    //$(".subLevelSelect").hover(function() {
    //},function() {
    //})

    $(".subLevelSelect,#quizBtn").click(function (e) {
        e.preventDefault();
        var locked = $(this).attr("data-locked");
        var front = $(this).attr("data-front");
        if (locked == "False") {
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