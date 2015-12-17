//Eğer kullanıcı 3 seviyeyi birden tek seferde geçerse 100 bonus kazanır
//Puan hesaplama  LevelPuan*seviye örn: level 1 için puan 5 olsun ilk sublevel(1*5) ikinci sublevel(2*5) üçüncü sublevel(3*5)
//eğer kullanıcı 3 yıldız aldıysa ve tekrar yapmak isterse    subLevel*1

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
    self.totalQuestions = ko.observable(self.dataQuestions().length);//sabit toplam soru
    self.totapInCorrect = ko.observable(0);// hata sayısı
    self.rate = ko.observable((1 / self.totalQuestions()) * 100);
    self.star = ko.observable(self.exams().Star);//yıldız sayısı
    self.updateWrapper = ko.observable(false);//updateWrapper
    self.loading = ko.observable(false);//loading
    self.textValue = ko.observable();//level 3'deki textboxdegeri
    self.fail = ko.observable(false); //başarısızlık durumu
    self.end = ko.observable(false);  //level tamamen bittiğinde güzükecek
    self.totalQuestions.subscribe(function (newValue) {
        self.rate((1 / self.totalQuestions()) * 100);
    });
    self.totalRate = ko.pureComputed(function () {
        return "% " + Math.ceil(((self.index()) / self.totalQuestions()) * 100);
    });

    //AltLevel tamamlandığında veritabanı güncelelme işlemi
    self.updateUserProggress = function () {
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
            }
        });
    }

    ///////////////////////////////////////////soru dizisi/////////////////////////////////////////
    self.Questions = {
        Question: ko.observable(),
        QuestionCorrect: ko.observable(),
        QestionList: ko.observableArray()
    };

    self.Questions.Question(self.dataQuestions()[self.index()].Question);
    self.Questions.QuestionCorrect(self.dataQuestions()[self.index()].QuestionCorrect);
    self.Questions.QestionList(self.dataQuestions()[self.index()].QestionsOptions);
    ///////////////////////////////////////////soru dizisi/////////////////////////////////////////

    self.successProgress = function (data) {
        var success = '<div class="progress-bar progress-bar-success" style="width: ' + data + '%"></div>';
        $(".progress").append(success);
    }
    self.errorProgress = function (data) {
        var success = '<div class="progress-bar progress-bar-danger" style="width: ' + data + '%"></div>';

        $(".progress").append(success);
    }
    //Soruya yanlış cevap verildiğinde hata ekranının gösterilmesi
    self.warning = function (correct, correct2) {
        var veri = '<button class="btn btn-lg btn-ques">' + "Cevap" + '&nbsp;&nbsp;<i class="fa fa-hand-o-right"></i>&nbsp;&nbsp;' + correct2 + '</button>';

        $("#warningWrapper").empty();
        $("#warningWrapper").append(veri);

        $(".btnsWrapper button").prop("disabled", true);

        self.warningAppear(true);
    }
    //Bir sonraki alt level soruları
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

    //SubLevellarda gösterilecek ekran
    self.lavelUpdate = function () {
        self.questionText(questionTextArray[self.subLevelNumber()]);
        self.okText(okTextArray[self.subLevelNumber() - 1]);

        if (self.star() <= self.subLevelNumber()) {
            self.star(self.star() + 1);
        }

        self.updateWrapper(true);
        self.updateUserProggress();

        if (self.subLevelNumber() > 2) {
            self.end(true);
        }
    };

    //Kullanıcı 3 hakkınıda bitirdiğinde soruları tekrar yükleme işlemi
    self.failBtn = function () {
        self.loading(true);

        var jsonData = { subLevel: self.subLevelNumber(), level: parseInt(levelNumber), kind: parseInt(kind) };

        $.ajax("/api/ajax/WordModulSubLevelQuestions", {
            type: "GET",
            data: jsonData,
            contentType: "application/json",
            success: function (data) {
                self.exams(data);
                self.subLevelNumber(self.exams().SubLevel);
                self.dataQuestions(self.exams().Questions);
                self.index(0);

                self.totalQuestions(self.dataQuestions().length);

                self.Questions.Question(self.dataQuestions()[self.index()].Question);
                self.Questions.QuestionCorrect(self.dataQuestions()[self.index()].QuestionCorrect);
                self.Questions.QestionList(self.dataQuestions()[self.index()].QestionsOptions);

                self.updateWrapper(false);
                self.warningAppear(false);
                $(".progress").empty();

                self.loading(false);

                self.fail(false);

                self.totapInCorrect(0);
                self.totalPuan(self.exams().TotalPuan);//toplan puan
                $(".btnsWrapper button").prop("disabled", false);
            }
        });
    }

    //kullanıcı hata yaptıktan sonra  bir sonraki soruya geçme
    self.nextBtn = function () {
        self.next();
        self.warningAppear(false);
        $(".btnsWrapper button").prop("disabled", false);
    }

    //Bir sonraki soruyu göster
    self.next = function () {
        self.index(self.index() + 1);
        if (self.index() + 9 < self.totalQuestions()) {
            self.textValue("");

            self.Questions.Question(self.dataQuestions()[self.index()].Question);
            self.Questions.QuestionCorrect(self.dataQuestions()[self.index()].QuestionCorrect);
            self.Questions.QestionList(self.dataQuestions()[self.index()].QestionsOptions);
        } else {
            if (self.totapInCorrect() <= 3) {
                self.lavelUpdate();
            }
        }
    }
    //Şıklardaki herhangi bir butona bastığında çalışır
    self.nextQuestion = function (data, event) {
        if ((data == self.Questions.QuestionCorrect()) || (self.textValue() == self.Questions.QuestionCorrect())) {
            self.successProgress(self.rate());
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

            self.totapInCorrect(self.totapInCorrect() + 1);
            if (self.totapInCorrect() >= 3) {
                self.fail(true);
            }
            var decrease = self.totalPuan() - self.subLevelNumber() * self.puan * 0.7;
            self.totalPuan(decrease);

            self.errorProgress(self.rate());
            self.warning(self.Questions.Question(), self.Questions.QuestionCorrect());
        }
    }
}