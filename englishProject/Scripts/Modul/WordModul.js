//Eğer kullanıcı 3 seviyeyi birden tek seferde geçerse 100 bonus kazanır
//Puan hesaplama  LevelPuan*seviye örn: level 1 için puan 5 olsun ilk sublevel(1*5) ikinci sublevel(2*5) üçüncü sublevel(3*5)
//eğer kullanıcı 3 yıldız aldıysa ve tekrar yapmak isterse    subLevel*1

var viewmodel = function (exams, levelId, levelSubLevel, boxId) {
    var self = this;
    self.toogle = true; //kartı  ters mi düz mü olduğunu anlamak için
    var card = 1; //kartı çevirdiğinde puanı yarıya düşürmek için  ilk başta 0 kart çevrilirse card=0.5 puan olmaktadır.
    var targetScore = 0; //kullanıcının günlük hedefi tesbit etmek için her alt seviyede  sıfırlanan puan
    var okTextArray = new Array("Temel seviye tamamlandı.", "İleri seviye tamamlandı.", "Mükemmel seviye tamamlandı.");
    var questionTextArray = new Array("Türkçe çevirisi nedir ?", "İngilizce çevirisi nedir ?", "Kelimenin ingilizce karşılığını yazınız.");
    self.subLevelCount = levelSubLevel;

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
    self.loading = ko.observable(false);//loading
    self.textValue = ko.observable("");//level 3'deki textboxdegeri

    //self.totalQuestions.subscribe(function (newValue) {
    //    self.rate((1 / self.totalQuestions()) * 100);
    //});
    //self.totalRate = ko.pureComputed(function () {
    //    return "% " + Math.ceil(((self.index()) / self.totalQuestions()) * 100);
    //});

    self.info1 = function () {
        var veri = "Kelime kartının sağ alt kısmında göreceğiniz <i class='fa fa-exchange'></i> işareti kelimenin  hatırlatma resimi olduğu anlamına gelir.  Karta tıklayarak hatırlatma resmini görebilirsiniz. Her tıklamada kelimeden kazanacağımız puanın<strong>yarısını</strong> kaybedersiniz.";
        messageShow(veri, 1);
    }
    self.info1();

    self.warningShow = ko.observable(false); //sorun yanlış cevap verildiğinde doğru cevabın ortay çıkmasını sağlar
    self.failShow = ko.observable(false); //başarısızlık durumu
    self.endShow = ko.observable(false);  //level tamamen bittiğinde güzükecek
    self.subLevelSuccessShow = ko.observable(false);//subLevelSuccessShow
    self.normalShow = ko.observable(true);

    self.normalAppear = function () {
        self.failShow(false);
        self.endShow(false);
        self.subLevelSuccessShow(false);
        self.warningShow(false);
        self.normalShow(true);
    }
    self.subLevelSuccessAppear = function () {
        self.failShow(false);
        self.endShow(false);
        self.normalShow(false);
        self.warningShow(false);
        self.subLevelSuccessShow(true);
    }
    self.endAppear = function () {
        self.subLevelSuccessShow(false);
        self.failShow(false);
        self.warningShow(false);
        self.normalShow(false);
        self.endShow(true);
    }
    self.failAppear = function () {
        self.normalShow(false);
        self.subLevelSuccessShow(false);
        self.endShow(false);
        self.warningShow(false);
        self.failShow(true);
    }

    //AltLevel tamamlandığında veritabanı güncelelme işlemi
    self.updateUserProggress = function () {
        self.userProgress = {
            levelId: parseInt(levelId),
            star: self.star(),
            puan: Math.round(self.totalPuan()),
            boxId: parseInt(boxId),
            targetScore: Math.round(targetScore)
        }
        var jsonData = ko.toJSON(self.userProgress);
        $.ajax("/api/ajax/UpdateUserProgress", {
            type: "POST",
            data: jsonData,
            contentType: "application/json",
            success: function (data) {
                targetScore = 0;
            }
        });
    }

    self.textValueCount = ko.pureComputed(function () {
        return self.textValue().length;
    });
    //kelimenin ingilice açıklaması olduğunu anlamak için
    self.definitionValueCount = ko.pureComputed(function () {
        if (self.Questions.Definition() != null) {
            return self.Questions.Definition().length;
        } else {
            return 0;
        }
    });
    //kelimenin ingilizce örnek cümlesi olduğunu anlamak için
    self.exampleValueCount = ko.pureComputed(function () {
        if (self.Questions.Example() != null) {
            return self.Questions.Example().length;
        } else {
            return 0;
        }
    });

    //kartıın bir hatırlatma kartı olduğunu anlama  anlamak için
    self.exChangeVisible = ko.pureComputed(function () {
        if (self.Questions.Remender() != null) {
            return self.Questions.Remender().length;
        } else {
            return 0;
        }
    });

    self.Questions = {
        Question: ko.observable(),
        Remender: ko.observable(""),
        Definition: ko.observable(""),
        Example: ko.observable(""),
        QuestionCorrect: ko.observable(),
        QestionList: ko.observableArray()
    };

    //dataları doldurmak için
    self.fill = function () {
        self.Questions.Question(self.dataQuestions()[self.index()].Question);
        self.Questions.Remender(self.dataQuestions()[self.index()].QuestionRemender);
        self.Questions.QuestionCorrect(self.dataQuestions()[self.index()].QuestionCorrect);
        self.Questions.QestionList(self.dataQuestions()[self.index()].QestionsOptions);
        self.Questions.Definition(self.dataQuestions()[self.index()].Definition);
        self.Questions.Example(self.dataQuestions()[self.index()].Example);
    }
    ///////////////////////////////////////////soru dizisi/////////////////////////////////////////

    self.fill();

    ///////////////////////////////////////////soru dizisi/////////////////////////////////////////

    self.Reset = function () {
        $("#questionCard").css("background-image", '');
        $("#questionCard").addClass("background").removeClass("succesQuestionCard").removeClass("warningQuestioCard").removeClass("questionCardReverse");
        $(".fa-check-circle").remove();
        $(".btnsWrapper button").removeClass("btn-update");
        $(".btnsWrapper button,#txtEnglish").prop("disabled", false);
        self.toogle = true;
        card = 1;
    }
    self.successProgress = function (data) {
        var success = '<div id="' + self.index() + '" class="progress-bar progress-bar-success" style="width: ' + 0 + '%"></div>';

        $(".progress").append(success);
        $("#" + self.index()).animate({
            width: data + "%",
        }, 100, "swing", function () {
            // Animation complete.
        });
    }
    self.errorProgress = function (data) {
        var success = '<div id="' + self.index() + '" class="progress-bar progress-bar-danger" style="width: ' + 0 + '%"></div>';

        $(".progress").append(success);

        $("#" + self.index()).animate({
            width: data + "%",
        }, 100, "swing", function () {
            // Animation complete.
        });
    }

    //Exhange

    self.exChange = function () {
        if (self.exChangeVisible() > 0) {
            if (self.toogle) {
                self.toogle = false;
                card = 0.5;
                $("#questionCard h1").empty().html("<i class='fa fa-spinner fa-pulse'></i>");
                $('<img>').attr('src', function () {
                    return self.Questions.Remender();
                }).load(function () {
                    $("#questionCard").addClass("questionCardReverse");

                    setTimeout(function () {
                        $("#questionCard h1").empty();
                        $("#questionCard").css("background-image", "url(" + self.Questions.Remender() + ")");
                    }, 300);
                });
            } else {
                self.toogle = true;
                $("#questionCard").removeClass("questionCardReverse");

                setTimeout(function () {
                    $("#questionCard").css("background-image", '');
                    $("#questionCard h1").text(self.Questions.Question());
                }, 300);
            }
        } else {
            return false;
        }
    }

    //Soruya yanlış cevap verildiğinde hata ekranının gösterilmesi
    self.warning = function (correct2) {
        var veri = '<button class="btn btn-lg btn-ques">' + "Cevap" + '&nbsp;&nbsp;<i class="fa fa-hand-o-right"></i>&nbsp;&nbsp;' + correct2 + '</button>';

        $("#warningWrapper").empty().append(veri);
        $(".btnsWrapper button,#txtEnglish").prop("disabled", true);

        self.warningShow(true);
        $("#btnContinous").focus();
    }

    //Bir sonraki alt level soruları
    self.updateBtn = function () {
        self.Reset();
        self.loading(true);

        var jsonData = { subLevel: self.subLevelNumber() + 1, levelId: parseInt(levelId) };

        $.ajax("/api/ajax/WordModulSubLevelQuestions", {
            type: "GET",
            data: jsonData,
            contentType: "application/json",
            success: function (data) {
                self.repeatData(data);
            },
            error: function () {
                alert("Diğer seviyenin yüklenmesinde hata meydana geldi. :( Sayfayı yenileyerek tekrar başlayınız...");
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

        self.updateUserProggress();

        if (self.subLevelNumber() < self.subLevelCount) {
            self.subLevelSuccessAppear();
        } else {
            self.endAppear();
        }
    };

    //Kullanıcı 3 hakkınıda bitirdiğinde soruları tekrar yükleme işlemi

    self.repeatData = function (data) {
        self.exams(data);
        self.subLevelNumber(self.exams().SubLevel);
        self.dataQuestions(self.exams().Questions);
        self.index(0);
        self.totalQuestions(self.dataQuestions().length);

        self.fill();
        $(".progress").empty();
        self.loading(false);

        $(".btnsWrapper button").prop("disabled", false);
        self.normalAppear();
    }

    self.failBtn = function () {
        self.loading(true);
        self.Reset();
        var jsonData = { subLevel: self.subLevelNumber(), levelId: parseInt(levelId) };

        $.ajax("/api/ajax/WordModulSubLevelQuestions", {
            type: "GET",
            data: jsonData,
            contentType: "application/json",
            success: function (data) {
                self.repeatData(data);
                self.totapInCorrect(0);
                self.totalPuan(self.exams().TotalPuan);//toplan puan
            },
            error: function () {
                alert("Diğer seviyenin yüklenmesinde hata meydana geldi. :( Sayfayı yenileyerek tekrar başlayınız...");
            }
        });
    }

    //kullanıcı hata yaptıktan sonra  bir sonraki soruya geçme
    self.nextBtn = function () {
        self.next();
        self.warningShow(false);
        $(".btnsWrapper button,#txtEnglish").prop("disabled", false);
    }

    //Bir sonraki soruyu göster
    self.next = function () {
        //ok işaretin ikaldır

        self.Reset();

        if (self.subLevelNumber() == 3) {
            $(".btnsWrapper button").text("Kontrol et");
            $("#txtEnglish").focus();
        }
        self.index(self.index() + 1);

        ////////////////////////////////////////////////////////////////////////////////////////////çabuk bitirme//////////////////////////////////////////////
        if (self.index() < self.totalQuestions()) {
            self.textValue("");

            self.fill();
        } else {
            if (self.totapInCorrect() <= 3) {
                self.lavelUpdate();
            }
        }
    }

    //Şıklardaki herhangi bir butona bastığında çalışır
    self.nextQuestion = function (data, event) {
        if ((data == self.Questions.QuestionCorrect()) || (self.textValue() == self.Questions.QuestionCorrect())) {
            $("#successAudi").trigger("play");

            if (self.subLevelNumber() < 3) {
                $(event.currentTarget).prepend("<i style='color:#3C5B2E' class='fa fa-check-circle'></i>&nbsp;");
                $(event.currentTarget).addClass("btn-update");
            }

            if (self.subLevelNumber() == 3) {
                $("#btnControl").prepend("<i style='color:#3C5B2E' class='fa fa-check-circle'></i>&nbsp;");
                $("#btnControl").addClass("btn-update");
                $("#btnControl").html("<i style='color:#3C5B2E' class='fa fa-check-circle'></i>&nbsp;Doğru");
            }

            $(".btnsWrapper button,#txtEnglish").prop("disabled", true);
            $("#questionCard h1").text("Doğru");
            $("#questionCard").css("background-image", '');
            $("#questionCard").removeClass("background").addClass("succesQuestionCard").removeClass("questionCardReverse");

            self.successProgress(self.rate());
            var increasePuan = 0;
            if (self.star() == 3) {
                //3 yıldızda 1 katsayısı çarpılır
                increasePuan = self.totalPuan() + self.subLevelNumber() * 1 * card;
                targetScore += self.subLevelNumber() * card;
            } else {
                //3 yıldız alınmadıysa level tablosunun levelPuan katsayısıyla çarpılır
                increasePuan = self.totalPuan() + self.subLevelNumber() * self.puan * card;
                targetScore += self.subLevelNumber() * self.puan * card;
            }
            self.totalPuan(increasePuan);
            setTimeout(function () { self.next(); }, 2000);
        } else {
            $("#wrongAudi").trigger("play");

            $("#questionCard h1").text("Yanlış");
            $("#questionCard").css("background-image", '');
            $("#questionCard").removeClass("background").addClass("warningQuestioCard").removeClass("questionCardReverse");
            //kalp sayısını arttır
            self.totapInCorrect(self.totapInCorrect() + 1);

            var decrease = Math.round(self.totalPuan() - self.subLevelNumber() * self.puan * 1);
            targetScore -= self.subLevelNumber() * self.puan * 1;
            self.totalPuan(decrease);

            self.errorProgress(self.rate());
            self.warning(self.Questions.QuestionCorrect());
            if (self.totapInCorrect() >= 3) {
                console.log("girdi");
                self.failAppear();
            }
        }
    }
}