ko.bindingHandlers.executeOnEnter = {
    init: function (element, valueAccessor, allBindings, viewModel) {
        var callback = valueAccessor();
        $(element).keypress(function (event) {
            var keyCode = (event.which ? event.which : event.keyCode);
            if (keyCode === 13) {
                callback.call(viewModel);
                return false;
            }
            return true;
        });
    }
}

$(document).ready(function () {
    $(window).load(function () {
        $("#loadingWrapper").hide();
        $("#levelExamWrapper").show();
    });

    $("#btnCommentIssue,#btnCommentIssue2").click(function () {
        $("#commentIssueModal").modal();
        $("#result").hide();
        $("#CommentIssue").val("");
    });
})