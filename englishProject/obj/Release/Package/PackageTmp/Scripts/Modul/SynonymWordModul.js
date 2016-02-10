var a;
var viewmodel = function (exams, levelId, levelSubLevel, boxId) {
    var self = this;

    var targetScore = 0; //kullanıcının günlük hedefi tesbit etmek için her alt seviyede  sıfırlanan puan
    var okTextArray = new Array("Temel seviye tamamlandı.", "İleri seviye tamamlandı.", "Mükemmel seviye tamamlandı.");
    var questionTextArray = new Array("Türkçe çevirisi nedir ?", "İngilizce çevirisi nedir ?", "Kelimenin ingilizce karşılığını yazınız.");
    self.subLevelCount = levelSubLevel;

    self.index = ko.observable(0);
    self.exams = ko.observable(exams);
    self.dataQuestions = ko.observableArray(self.exams().Questions);
    self.okText = ko.observable();
    self.questionText = ko.observable(questionTextArray[self.exams().SubLevel - 1]);
    self.puan = self.exams().Puan;
    self.subLevelNumber = ko.observable(self.exams().SubLevel);

    self.totalPuan = ko.observable(self.exams().TotalPuan);
    self.totalQuestions = ko.observable(self.dataQuestions().length);
    self.totapInCorrect = ko.observable(0);
    self.rate = ko.observable(Math.floor((5 / self.totalQuestions()) * 100));
    self.star = ko.observable(self.exams().Star);

    //////////////////////////////////////////////////golabal  değişkenler////////////////////////////////////////////////
    self.warningShow = ko.observable(false);
    self.failShow = ko.observable(false);
    self.endShow = ko.observable(false);
    self.subLevelSuccessShow = ko.observable(false);
    self.normalShow = ko.observable(true);
    self.loading = ko.observable(false);
    //////////////////////////////////////////////////golabal  değişkenler////////////////////////////////////////////////
    self.info1 = function () {
        var veri = "Kelime kartının sağ alt kısmında göreceğiniz <i class='fa fa-exchange'></i> işareti kelimenin  hatırlatma resimi olduğu anlamına gelir.  Karta tıklayarak hatırlatma resmini görebilirsiniz. Her tıklamada kelimeden kazanacağımız puanın<strong>yarısını</strong> kaybedersiniz.";
        messageShow(veri, 2);
    }
    self.info1();

    //////////////////////////////ozel değişken///////////////////////////
    a = self.subLevelNumber(); //a değeri golabal değişken
    var totalSuccess = 0; // bir sorudaki toplam başarı durumu  başarı için 5 de 5 yapması lazım
    self.turkishVisible = ko.observable(false);
    self.continousShow = ko.observable(false);
    self.controlShow = ko.observable(true);

    //////////////////////////////ozel değişken///////////////////////////

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

    self.QuestionsList = ko.observableArray([]);
    self.RandomQuestionsList = ko.observableArray([]);
    self.Questions = {
        Key: ko.observable(),
        Synonym1: ko.observable(),
        Synonym2: ko.observable(),
        Turkish: ko.observable()
    };

    self.repeatData = function (data) {
        self.index(0);
        self.exams(data);
        self.subLevelNumber(self.exams().SubLevel);
        self.dataQuestions(self.exams().Questions);
        self.totalQuestions(self.dataQuestions().length);
        self.fill();
        $(".progress").empty();
        self.loading(false);

        self.normalAppear();
    }

    self.failBtn = function () {
        self.loading(true);
        self.reset();
        var jsonData = { subLevel: self.subLevelNumber(), levelId: parseInt(levelId) };

        $.ajax("/api/ajax/SynonymModulSubLevelQuestions", {
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
    self.updateBtn = function () {
        self.reset();
        self.loading(true);

        var jsonData = { subLevel: self.subLevelNumber() + 1, levelId: parseInt(levelId) };

        $.ajax("/api/ajax/SynonymModulSubLevelQuestions", {
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

    self.increase = function () {
        var increasePuan = 0;
        if (self.star() == 3) {
            //3 yıldızda 1 katsayısı çarpılır
            increasePuan = self.totalPuan() + self.subLevelNumber() * 1;
            targetScore += self.subLevelNumber();
        } else {
            //3 yıldız alınmadıysa level tablosunun levelPuan katsayısıyla çarpılır
            increasePuan = self.totalPuan() + self.subLevelNumber() * self.puan;
            targetScore += self.subLevelNumber() * self.puan;
        }
        self.totalPuan(increasePuan);
    }

    self.decrease = function () {
        self.totapInCorrect(self.totapInCorrect() + 1);
        var decrease = Math.round(self.totalPuan() - self.subLevelNumber() * self.puan * 1);
        targetScore -= self.subLevelNumber() * self.puan * 1;
        self.totalPuan(decrease);
    }

    self.failStatus = function () {
        if (self.totapInCorrect() >= 3) {
            self.failAppear(true);
        }
    }
    self.reset = function () {
        totalSuccess = 0;
        self.QuestionsList([]);
        self.turkishVisible(false);
        self.controlShow(true);
        self.continousShow(false);
    }

    self.backBtn = function (data, e) {
        console.log(e.currentTarget);

        var id = $(e.currentTarget).parent().parent().attr("data-key");
        $("#group-" + id).removeClass("active");

        var btn = $(e.currentTarget).parent("div");
        btn.children("i").hide();
        btn.children("a").hide();
        $("#rightWrapper").append(btn);
    }

    self.fill = function () {
        $("#rightWrapper .btn").tooltip('hide');

        console.log(self.index() + "<" + self.dataQuestions().length);
        if (self.index() < self.dataQuestions().length) {
            do {
                self.QuestionsList.push(self.dataQuestions()[self.index()]);

                self.index(self.index() + 1);
            } while (self.index() < self.dataQuestions().length && self.index() % 5 != 0)
        } else {
            alert("bitti");
        }

        if (self.subLevelNumber() == 1) {
        }
    }

    self.fill();

    self.contionous = function () {
        self.reset();

        if (self.index() < self.totalQuestions()) {
            self.fill();
        } else {
            if (self.totapInCorrect() <= 3) {
                self.levelUpdate();
            }
        }
    }
    self.levelUpdate = function () {
        //self.questionText(questionTextArray[self.subLevelNumber()]);
        self.okText(okTextArray[self.subLevelNumber() - 1]);

        if (self.star() < self.subLevelNumber()) {
            self.star(self.star() + 1);
        }

        self.updateUserProggress();

        if (self.subLevelNumber() > 1) {
            $("#rightWrapper .btn").tooltip('hide');
        }

        if (self.subLevelNumber() < self.subLevelCount) {
            self.subLevelSuccessAppear();
        } else {
            self.endAppear();
        }
    };
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

    self.control = function (e) {
        var righBtns = $("#rightWrapper .btn");

        if (righBtns.length > 0) {
            $.each(righBtns, function (index, value) {
                $(value).removeClass("animated shake").addClass("animated shake").one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
                    $(this).removeClass("animated shake");
                });
            });
        } else {
            self.continousShow(true);
            self.controlShow(false);
            self.turkishVisible(true);
            $(".btn a").hide();
            $.each($(".group"), function (index, value) {
                var first = $(value).children().eq(0);
                var second = $(value).children().eq(1);
                console.log(first + "--" + second);

                if (first.attr("data-key") == second.attr("data-key")) {
                    first.removeClass("btn-primary").addClass("btn-success");
                    second.removeClass("btn-info").addClass("btn-success");
                    totalSuccess++;
                } else {
                    //second.attr("data-toggle", "tooltip");
                    //second.attr("data-placement", "right");
                    second.attr("data-original-title", "cevap:" + self.QuestionsList()[index].Synonym2);
                    second.attr("title", "cevap:" + self.QuestionsList()[index].Synonym2);
                    first.removeClass("btn-primary").addClass("btn-danger");
                    second.removeClass("btn-info").addClass("btn-danger");
                }
            });

            if (totalSuccess == 5) {
                self.successProgress(self.rate());
                self.increase();
            } else {
                self.errorProgress(self.rate());
                self.decrease();
                self.failStatus();
            }
            $('[data-toggle="tooltip"]').tooltip();
        }
    }
}
////////////////////////////////////////////////////////// drag end drop/////////////////////////////////////////////
$(document).ready(function () {
    $(window).load(function () {
        if (a == 1) {
            $('[data-toggle="tooltip"]').tooltip();
        }
    });
});

function noDrop(e) {
    e.stopPropagation();
}

function dragover(e) {
    e.preventDefault();

    e.dataTransfer.dropEffect = "copy";
}

function dragleave(e) {
    e.preventDefault();
    var id;

    if ($(e.target).parent().is(".group")) {
        id = $(e.target).parent().attr("data-key");
    } else {
        id = $(e.target).attr("data-key");
    }

    $("#group-" + id).removeClass("active");
}

function dragenter(e) {
    e.preventDefault();
    var id;

    if ($(e.target).parent().is(".group")) {
        id = $(e.target).parent().attr("data-key");
    } else {
        id = $(e.target).attr("data-key");
    }

    $("#group-" + id).addClass("active");
}

function drop(e) {
    e.preventDefault();

    var droppedID = e.dataTransfer.getData("text");

    var btn = $("#" + droppedID);

    if (e.target.id == "rightWrapper" || $(e.target).parent("#rightWrapper").is("div")) {
        btn.children("i").hide();
        btn.children("a").hide();

        $("#group-" + btn.parent().attr("data-key")).removeClass("active");
        $("#rightWrapper").append(btn);
    } else {
        var id;

        if ($(e.target).parent(".group").is("div")) {
            id = $(e.target).parent(".group").attr("data-key");
            if ($(e.target).parent(".group").children("div").length == 1) {
                var previousBtn = $(e.target);

                btn.parent().append(previousBtn);

                if (btn.parent().attr("id") == "rightWrapper") {
                    previousBtn.children("i").hide();
                    previousBtn.children("a").hide();
                }
            }
        } else {
            id = e.target.id.split("-")[1];
        }

        btn.children("i").addClass("animated zoomIn").show();
        btn.children("a").show();
        $("#group-" + id).append(btn);
        btn.addClass("animated bounceIn");
    }
}

function dragstart(e) {
    e.dataTransfer.effectAllowed = "copy";
    e.dataTransfer.setData("text", e.target.id);
}

$(document).ready(function () {
    //alert(Modernizr.draganddrop);
});