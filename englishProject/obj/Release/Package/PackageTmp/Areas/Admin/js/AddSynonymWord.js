var SynonymWordModel = function () {
    var self = this;

    self.synonymWord = {
        levelId: ko.observable(),
        synonymTurkish: ko.observable(),
        synonym1: ko.observable(),
        synonym2: ko.observable(),
        synonym3: ko.observable()
    }
    self.loading = ko.observable(false);
    self.words = ko.observableArray([]);

    self.total = ko.pureComputed(function () {
        return self.words().length;
    }, this);

    self.wordsReverse = ko.pureComputed(function () {
        return self.words().reverse();
    });

    self.deleteWord = function (item) {
        var jsonData = ko.toJSON({ synonymId: item.id });

        $.ajax("/SynonymWord/DeleteSynonymWord", {
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
        var jsonData = ko.toJSON(self.synonymWord);

        $.ajax("/SynonymWord/CreateSynonymWord", {
            type: "POST",
            data: jsonData,
            contentType: "application/json",
            success: function (data) {
                console.log(data);
                var json = data.split("-");

                if (json[0] == "True") {
                    self.words.push({ id: json[1], turkish: self.synonymWord.synonymTurkish(), translate1: self.synonymWord.synonym1(), translate2: self.synonymWord.synonym2(), translate3: self.synonymWord.synonym3() });

                    self.synonymWord.synonymTurkish("");
                    self.synonymWord.synonym1("");
                    self.synonymWord.synonym2("");
                    self.synonymWord.synonym3("");
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
    var model = new SynonymWordModel();
    ko.applyBindings(model);
});