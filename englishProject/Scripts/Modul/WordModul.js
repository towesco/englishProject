var viewmodel = function (exams, levelNumber, kind, boxNumber) {
    var self = this;
    //var levelNumber = levelNumber;
    //var kind = kind;
    //var boxNumber = boxNumber;
    var okTextArray = new Array("Temel seviye tamamlandı.", "İleri seviye tamamlandı.", "Mükemmel seviye tamamlandı.");
    var questionTextArray = new Array("Türkçe çevirisi nedir ?", "İngilizce çevirisi nedir ?", "Kelimenin ingilizce karşılığını yazınız.");
    self.warningAppear = ko.observable(false); //sorun yanlış cevap verildiğinde doğru cevabın ortay çıkmasını sağlar
    self.index = ko.observable(0); //0 indexli kayıtı getirir
    self.exams = ko.observable(exams); //datanın tamamını çeker;
    self.dataQuestions = ko.observableArray(self.exams().Questions);//şıklar
    self.okText = ko.observable();//seviye tamamlandığında yazan yazı
    self.questionText = ko.observable(questionTextArray[self.exams().SubLevel - 1]); //  sub level ait soru cumlesi
    self.puan = self.exams().Puan;//sabit level puanı
    self.subLevelNumber = ko.observable(self.exams().SubLevel);//değişken sub level numarası
    self.totalPuan = ko.observable(self.exams().TotalPuan);//toplan puan
    self.currentQuestion = ko.observable(1);//güncel soru
    self.totalQuestions = ko.observable(self.dataQuestions().length);//sabit toplam soru
    self.totapInCorrect = ko.observable(0);// hata sayısı
    self.rate = (self.currentQuestion() / self.totalQuestions()) * 100;//sorunun yüzdelik karşılığı
    self.star = ko.observable(self.exams().Star);//yıldız sayısı
    self.updateWrapper = ko.observable(false);//updateWrapper
    self.loading = ko.observable(false);//loading
    self.textValue = ko.observable();//level 3'deki textboxdegeri
    self.updateUserProggress = function () {
        if (self.star() > 3) {
            self.star(3);
        }
        self.userProgress = {
            levelNumber: parseInt(levelNumber),
            kind: parseInt(kind),
            star: self.star(),
            puan: Math.round(self.totalPuan()),
            boxNumber: boxNumber
        }
        var jsonData = ko.toJSON(self.userProgress);
        $.ajax("/api/ajax/UpdateUserProgress", {
            type: "POST",
            data: jsonData,
            contentType: "application/json",
            success: function (data) {
                alert(data);
            }
        });
    }
    self.Questions = {
        Question: ko.observable(),
        QuestionCorrect: ko.observable(),
        QestionList: ko.observableArray()
    };

    self.Questions.Question(self.dataQuestions()[self.index()].Question);
    self.Questions.QuestionCorrect(self.dataQuestions()[self.index()].QuestionCorrect);
    self.Questions.QestionList(self.dataQuestions()[self.index()].QestionsOptions);
    self.successProgress = function (data) {
        var success = '<div class="progress-bar progress-bar-success" style="width: ' + data + '%"></div>';

        $(".progress").append(success);
    }
    self.errorProgress = function (data) {
        var success = '<div class="progress-bar progress-bar-danger" style="width: ' + data + '%"></div>';

        $(".progress").append(success);
    }
    self.warning = function (correct, correct2) {
        var veri = '<button class="btn btn-success btn-lg ">' + correct + '&nbsp;&nbsp;<i class="fa fa-hand-o-right"></i>&nbsp;&nbsp;' + correct2 + '</button>';

        $("#warningWrapper").empty();
        $("#warningWrapper").append("<strong>Doğru cevap:&nbsp;&nbsp;</strong>");
        $("#warningWrapper").append(veri);

        $("#btnsWrapper button").prop("disabled", true);

        self.warningAppear(true);
    }
    self.updateBtn = function () {
        self.loading(true);

        var jsonData = { subLevel: self.subLevelNumber() + 1, level: parseInt(levelNumber), kind: parseInt(kind) };

        $.ajax("/api/ajax/WordModulSubLevelQuestions", {
            type: "GET",
            data: jsonData,
            contentType: "application/json",
            success: function (data) {
                self.exams(data);
                self.subLevelNumber(self.exams().SubLevel);
                self.dataQuestions(self.exams().Questions);
                self.index(0);
                self.currentQuestion(1);
                self.totalQuestions(self.dataQuestions().length);

                self.Questions.Question(self.dataQuestions()[self.index()].Question);
                self.Questions.QuestionCorrect(self.dataQuestions()[self.index()].QuestionCorrect);
                self.Questions.QestionList(self.dataQuestions()[self.index()].QestionsOptions);
                self.updateWrapper(false);
                $(".progress").empty();
                self.loading(false);
            }
        });
    }
    self.lavelUpdate = function () {
        self.questionText(questionTextArray[self.subLevelNumber()]);
        self.okText(okTextArray[self.subLevelNumber() - 1]);
        self.star(self.star() + 1);
        self.updateWrapper(true);
        self.updateUserProggress();
    };
    self.next = function () {
        if (self.currentQuestion() + 8 < self.totalQuestions()) {
            self.textValue("");
            var i = self.index() + 1;
            self.index(i); // index 0 dan 1 çıkıyor
            self.currentQuestion(self.index() + 1); // soru 1 den 2'ye çıkıyorr

            self.Questions.Question(self.dataQuestions()[self.index()].Question);
            self.Questions.QuestionCorrect(self.dataQuestions()[self.index()].QuestionCorrect);
            self.Questions.QestionList(self.dataQuestions()[self.index()].QestionsOptions);
        } else {
            if (self.totapInCorrect() < 4) {
                self.lavelUpdate();
            }
        }
    }
    self.nextBtn = function () {
        self.next();
        self.warningAppear(false);
        $("#btnsWrapper button").prop("disabled", false);
    }
    self.nextQuestion = function (data, event) {
        if ((data == self.Questions.QuestionCorrect()) || (self.textValue() == self.Questions.QuestionCorrect())) {
            self.successProgress(self.rate);
            var increasePuan = 0;
            if (self.star() == 3) {
                //3 yıldızda 1 katsayısı çarpılır
                increasePuan = self.totalPuan() + self.subLevelNumber() * 1;
            } else {
                //3 yıldız alınmadıysa level tablosunun levelPuan katsayısıyla çarpılır
                increasePuan = self.totalPuan() + self.subLevelNumber() * self.puan;
            }
            self.totalPuan(increasePuan);
            self.next();
        } else {
            //kalp sayısını arttır
            var incorrect = self.totapInCorrect() + 1;
            if (incorrect <= 3) {
                self.totapInCorrect(incorrect);
            } else {
                //alert("3 yanlış oldu");
            }

            var decrease = self.totalPuan() - self.subLevelNumber() * self.puan * 0.7;
            self.totalPuan(decrease);

            self.errorProgress(self.rate);
            self.warning(self.Questions.Question(), self.Questions.QuestionCorrect());
        }
    }
}