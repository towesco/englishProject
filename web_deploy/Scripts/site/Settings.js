var userProfilView = function (data) {
    var self = this;
    self.totalPuan = data.TotalPuan;
    self.UserProfilBoxs = data.UserProfilBoxs;

    for (var i = 0; i < self.UserProfilBoxs.length; i++) {
        $("#circleLevel").append("<div class='pull-left' style='width:180px;margin:10px;'><h4>" + self.UserProfilBoxs[i].boxName + "</h4><div id='" + i + "' style=' width: 150px; margin:0 auto;  height: 150px;'></div><h4>" + self.UserProfilBoxs[i].CurrentLevel + ".Seviye</h4></div>");

        var parcent = self.UserProfilBoxs[i].CurrentProgress / self.UserProfilBoxs[i].Progress;
        //$("#" + i).append("<h1>" + parcent.toFixed(2) + "</h1>");

        var element = document.getElementById(i);
        var circle = new ProgressBar.Circle(element, {
            color: '#283048',
            strokeWidth: 15,
            trailWidth: 10,
            trailColor: '#BAE6F2',
            duration: 1500,
            text: {
                value: '%0'
            },
            step: function (state, bar) {
                var yuzde = Math.abs((bar.value() * 100).toFixed(0));
                bar.setText("%" + yuzde);
            }
        });
        circle.animate(parcent, function () {
        });
    }
}

var siteSettingsView = function (data) {
    var self = this;

    self.data = ko.observable(data);

    var soundEffect = self.data().SoundEffect ? "true" : "false";

    self.SoundEffect = ko.observable(soundEffect);

    self.SoundEffect.subscribe(function (newValue) {
        self.data().SoundEffect = newValue;

        self.update();
    });

    self.update = function () {
        var jsonData = ko.toJSON(self.data());

        $.ajax("/api/ajax/UpdateUserDetail", {
            type: "POST",
            data: jsonData,
            contentType: "application/json",
            success: function (ajaxData) {
                SuccessMessageShow(ajaxData);
            },
            error: function () {
                GetErrorMessage();
            }
        });
    }
}