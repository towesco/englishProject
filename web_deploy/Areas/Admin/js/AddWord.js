var WordModel = function () {
    var self = this;

    self.Word = {
        wordTurkish: ko.observable(),
        wordTranslate: ko.observable(),
        levelId: ko.observable(),
        info: ko.observable(),
        wordRemender: ko.observable(""),
        wordRemenderInfo: ko.observable(""),
        wordDefinition: ko.observable(""),
        wordExample: ko.observable("")
    }
    self.loading = ko.observable(false);
    self.words = ko.observableArray([]);

    self.total = ko.pureComputed(function () {
        return self.words().length;
    }, this);

    self.wordsReverse = ko.pureComputed(function () {
        return self.words().reverse();
    });

    $('#wordRemender,#wordRemenderInfo').fileupload({
        add: function (e, data) {
            data.submit().success(function (result, textStatus, abc) {
                var jsonData = ko.toJSON({ path: $(e.target).parent().children("img").first().attr("src") });
                $.ajax("/word/DeletePicture", {
                    type: "POST",
                    data: jsonData,
                    contentType: "application/json",
                    success: function (s) {
                    }
                });

                $(e.target).parent().children("img").remove();
                $("<img/>").attr("src", result).addClass("img-thumbnail").appendTo($(e.target).parent());

                if (e.target.id == "wordRemender") {
                    self.Word.wordRemender(result);
                } else {
                    self.Word.wordRemenderInfo(result);
                }
            });
        },
        fail: function (e, data) {
            alert("hata var");
        },

        done: function (e, data) {
        }
    });

    self.deleteWord = function (item) {
        var jsonData = ko.toJSON({ wordId: item.id });

        $.ajax("/word/DeleteWord", {
            type: "POST",
            data: jsonData,
            contentType: "application/json",
            success: function (data) {
                if (data) {
                    self.words.remove(item);
                }
            }
        });
    }

    self.createWord = function (form) {
        self.loading(true);
        var jsonData = ko.toJSON(self.Word);

        $.ajax("/word/CreateWord", {
            type: "POST",
            data: jsonData,
            contentType: "application/json",
            success: function (data) {
                console.log(data);
                var json = data.split("-");

                if (json[0] == "True") {
                    self.words.push({ id: json[1], turkish: self.Word.wordTurkish(), translate: self.Word.wordTranslate(), number: self.Word.levelId(), info: self.Word.info(), remender: self.Word.wordRemender(), remenderInfo: self.Word.wordRemenderInfo(), definition: self.Word.wordDefinition(), example: self.Word.wordExample() });
                    self.Word.wordTurkish("");
                    self.Word.wordTranslate("");
                    self.Word.info("");
                    self.Word.wordRemender("");
                    self.Word.wordRemenderInfo("");
                    self.Word.wordDefinition("");
                    self.Word.wordExample("");

                    $("#images img").remove();
                } else {
                    alert("Bu kelimeyi daha önce girdin");
                }
            },
            complete: function () {
                self.loading(false);
            }
        });

        return false;
    };
}

$(document).ready(function () {
    var model = new WordModel();
    ko.applyBindings(model);
});