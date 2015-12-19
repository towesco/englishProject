var WordModel = function () {
    var self = this;

    self.Word = {
        wordTurkish: ko.observable(),
        wordTranslate: ko.observable(),
        levelNumber: ko.observable(),
        kind: ko.observable(),
        picture: ko.observable(),
        info: ko.observable()
    }
    self.loading = ko.observable(false);
    self.words = ko.observableArray([]);

    self.total = ko.pureComputed(function () {
        return self.words().length;
    }, this);

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
                self.words.push({ id: data, a: self.Word.wordTurkish(), b: self.Word.wordTranslate(), c: self.Word.levelNumber(), d: self.Word.kind(), e: self.Word.picture(), f: self.Word.info() });
                self.Word.wordTurkish("");
                self.Word.wordTranslate("");
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