var viewmodel = function (exams, levelNumber, kind, boxNumber) {
    var self = this;

    var okTextArray = new Array("Temel seviye tamamlandı.", "İleri seviye tamamlandı.", "Mükemmel seviye tamamlandı.");
    var questionTextArray = new Array("Kırmızı işeretli yer vücudun neresidir ?", "Kırmızı işretli yeri kutucuğa yazınız ?");
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
    self.textValue = ko.observable("");//level 3'deki textboxdegeri
    self.fail = ko.observable(false); //başarısızlık durumu
    self.end = ko.observable(false);  //level tamamen bittiğinde güzükecek

    ///////////////////////////////////////////soru dizisi/////////////////////////////////////////

    self.Questions = {
        x: ko.observable(),
        y: ko.observable(),
        Info: ko.observable(),
        Picture: ko.observable(),
        Correct: ko.observable(),
        List: ko.observableArray(),
    };

    self.Questions.Info(self.dataQuestions()[self.index()].QuestionInfo);
    self.Questions.Picture(self.dataQuestions()[self.index()].QuestionPicture);
    self.Questions.Correct(self.dataQuestions()[self.index()].QuestionCorrect);
    self.Questions.List(self.dataQuestions()[self.index()].QestionsOptions);
    self.Questions.x(self.Questions.Info().split(",")[0] + "px");
    self.Questions.y(self.Questions.Info().split(",")[1] + "px");
    ///////////////////////////////////////////soru dizisi/////////////////////////////////////////

    self.textValueCount = ko.pureComputed(function () {
        return self.textValue().length;
    });
    self.backgroundImg = ko.pureComputed(function () {
        var split = self.Questions.Info().split(",");

        return split[2] + "px " + split[3] + "px";
    });

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
    self.successProgress = function (data) {
        var success = '<div class="progress-bar progress-bar-success" style="width: ' + data + '%"></div>';
        $(".progress").append(success);
    }
    self.errorProgress = function (data) {
        var success = '<div class="progress-bar progress-bar-danger" style="width: ' + data + '%"></div>';

        $(".progress").append(success);
    }
    //Soruya yanlış cevap verildiğinde hata ekranının gösterilmesi
    self.warning = function (correct2) {
        var veri = '<button class="btn btn-lg btn-ques">' + "Cevap" + '&nbsp;&nbsp;<i class="fa fa-hand-o-right"></i>&nbsp;&nbsp;' + correct2 + '</button>';

        $("#warningWrapper").empty();
        $("#warningWrapper").append(veri);

        $(".btnsWrapper button,#txtEnglish").prop("disabled", true);

        $("#txtEnglish").blur();
        self.warningAppear(true);
        $("#btnContinous").focus();
    }
    //Bir sonraki alt level soruları
    self.updateBtn = function () {
        self.loading(true);

        var jsonData = { subLevel: self.subLevelNumber() + 1, level: parseInt(levelNumber), kind: parseInt(kind) };

        $.ajax("/api/ajax/PictureWordModulSubLevelQuestions", {
            type: "GET",
            data: jsonData,
            contentType: "application/json",
            success: function (data) {
                self.exams(data);
                self.subLevelNumber(self.exams().SubLevel);
                self.dataQuestions(self.exams().Questions);
                self.index(0);

                self.totalQuestions(self.dataQuestions().length);

                self.Questions.Info(self.dataQuestions()[self.index()].QuestionInfo);
                self.Questions.Picture(self.dataQuestions()[self.index()].QuestionPicture);
                self.Questions.Correct(self.dataQuestions()[self.index()].QuestionCorrect);
                self.Questions.List(self.dataQuestions()[self.index()].QestionsOptions);
                self.Questions.x(self.Questions.Info().split(",")[0] + "px");
                self.Questions.y(self.Questions.Info().split(",")[1] + "px");

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

        if (self.star() < self.subLevelNumber()) {
            self.star(self.star() + 1);
        }

        self.updateWrapper(true);
        self.updateUserProggress();

        if (self.subLevelNumber() > 1) {
            self.end(true);
        }
    };

    //Kullanıcı 3 hakkınıda bitirdiğinde soruları tekrar yükleme işlemi
    self.failBtn = function () {
        self.loading(true);

        var jsonData = { subLevel: self.subLevelNumber(), level: parseInt(levelNumber), kind: parseInt(kind) };

        $.ajax("/api/ajax/PictureWordModulSubLevelQuestions", {
            type: "GET",
            data: jsonData,
            contentType: "application/json",
            success: function (data) {
                self.exams(data);
                self.subLevelNumber(self.exams().SubLevel);
                self.dataQuestions(self.exams().Questions);
                self.index(0);

                self.totalQuestions(self.dataQuestions().length);

                self.Questions.Info(self.dataQuestions()[self.index()].QuestionInfo);
                self.Questions.Picture(self.dataQuestions()[self.index()].QuestionPicture);
                self.Questions.Correct(self.dataQuestions()[self.index()].QuestionCorrect);
                self.Questions.List(self.dataQuestions()[self.index()].QestionsOptions);
                self.Questions.x(self.Questions.Info().split(",")[0] + "px");
                self.Questions.y(self.Questions.Info().split(",")[1] + "px");

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
        $(".btnsWrapper button,#txtEnglish").prop("disabled", false);
    }

    //Bir sonraki soruyu göster
    self.next = function () {
        //ok işaretin ikaldır
        $(".fa-check-circle").remove();
        $(".btnsWrapper button").removeClass("btn-update");
        if (self.subLevelNumber() == 2) {
            $(".btnsWrapper button").text("Kontrol et");
        }
        self.index(self.index() + 1);
        if (self.index() < self.totalQuestions()) {
            self.textValue("");

            self.Questions.Info(self.dataQuestions()[self.index()].QuestionInfo);
            self.Questions.Picture(self.dataQuestions()[self.index()].QuestionPicture);
            self.Questions.Correct(self.dataQuestions()[self.index()].QuestionCorrect);
            self.Questions.List(self.dataQuestions()[self.index()].QestionsOptions);
            self.Questions.x(self.Questions.Info().split(",")[0] + "px");
            self.Questions.y(self.Questions.Info().split(",")[1] + "px");
        } else {
            if (self.totapInCorrect() <= 3) {
                self.lavelUpdate();
            }
        }
    }
    //Şıklardaki herhangi bir butona bastığında çalışır
    self.nextQuestion = function (data, event) {
        if ((data == self.Questions.Correct()) || (self.textValue() == self.Questions.Correct())) {
            if (self.subLevelNumber() < 2) {
                $(event.currentTarget).prepend("<i style='color:#3C5B2E' class='fa fa-check-circle'></i>&nbsp;");
                $(event.currentTarget).addClass("btn-update");
            }

            if (self.subLevelNumber() == 2) {
                $("#btnControl").prepend("<i style='color:#3C5B2E' class='fa fa-check-circle'></i>&nbsp;");
                $("#btnControl").addClass("btn-update");
                $("#btnControl").html("<i style='color:#3C5B2E' class='fa fa-check-circle'></i>&nbsp;Doğru");
            }

            self.successProgress(self.rate());
            var increasePuan = 0;
            if (self.star() == 2) {
                //3 yıldızda 1 katsayısı çarpılır
                increasePuan = self.totalPuan() + self.subLevelNumber() * 1;
            } else {
                //3 yıldız alınmadıysa level tablosunun levelPuan katsayısıyla çarpılır
                increasePuan = self.totalPuan() + self.subLevelNumber() * self.puan;
            }
            self.totalPuan(increasePuan);
            setTimeout(function () { self.next(); }, 2000);
        } else {
            //kalp sayısını arttır

            self.totapInCorrect(self.totapInCorrect() + 1);
            if (self.totapInCorrect() >= 3) {
                self.fail(true);
            }
            var decrease = Math.round(self.totalPuan() - self.subLevelNumber() * self.puan * 0.7);
            self.totalPuan(decrease);

            self.errorProgress(self.rate());
            self.warning(self.Questions.Correct());
        }
    }
}