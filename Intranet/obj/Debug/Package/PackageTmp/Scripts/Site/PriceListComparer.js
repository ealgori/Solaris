$(function () {
    $("#Sendbtn").click(function (e) {
        e.preventDefault();
        var firstChecked = $("#firstSubcPLs").find('input:checked');
        var secondChecked = $("#secondSubcPLs").find('input:checked');
        if ((firstChecked.length !== 0) && (secondChecked.length !== 0)) {
            $("#compareForm").submit();
        }
    });
    var firstSubcPLsSelector = "#firstSubcPLs";
    var secondSubcPLsSelector = "#secondSubcPLs";
    $("#firstSubc").width(430).kendoComboBox({ filter: "contains", placeholder: "Выбор подрядчика ...", suggest: true });
    $("#secondSubc").width(430).kendoComboBox({ filter: "contains", placeholder: "Выбор подрядчика ...", suggest: true });
    $("#firstSubc").data("kendoComboBox").text("");
    $("#secondSubc").data("kendoComboBox").text("");

    $("#firstSubc").change(function () {
        var n = $(this).val();
        setOpacity("table",0.2);
        $(firstSubcPLsSelector).empty();
        tableToBlackColor();
        $.getJSON(getPriceListsUrl + '?' + Math.random(), { subcId: n })
        .done(function (json) {


            for (var i = 0; i < json.length; i++) {
                DrawPriceListRow(firstSubcPLsSelector, "fpl", json[i]);
            };

            $(firstSubcPLsSelector + " input:radio").change(function () {
                tableToBlackColor();
                var value = $(this).val();
                var subc = $("#secondSubc").val();
                if (subc) {
                    $.getJSON(crossedItemUrl + '?' + Math.random(), { plId: value, subcId: subc })
                   
                    .done(function (js) {
                        // рисование на правой табличке с пересекающимися айтемами
                        for (var i = 0; i < json.length; i++) {
                            var tr = $(secondSubcPLsSelector + " input:radio[value=" + js[i] + "]").closest("tr");
                            tr.css("color", "rgb(15, 134, 36)");

                            tr.css("font-weight", "bold");

                        }


                    })
                     
                }
            });

        })
  .fail(function (jqxhr, textStatus, error) {
      var err = textStatus + ", " + error;
      console.log("Request Failed: " + err);
  })
  .always(function () {
      setOpacity("table", 1);
  })

    });

    function tableToBlackColor() {

        $(secondSubcPLsSelector + " tr").css("color", "rgb(0, 0, 0)");
        $(secondSubcPLsSelector + " tr").css("font-weight", "normal");

    }

    $("#secondSubc").change(function () {
        var n = $(this).val();

        $(secondSubcPLsSelector).empty();
        setOpacity("table", 0.2);
        tableToBlackColor();
        $.getJSON(getPriceListsUrl + '?' + Math.random(), { subcId: n })
       .done(function (json) {

           for (var i = 0; i < json.length; i++) {
               DrawPriceListRow(secondSubcPLsSelector, "spl", json[i]);
           };


       })
 .fail(function (jqxhr, textStatus, error) {
     var err = textStatus + ", " + error;
     console.log("Request Failed: " + err);
 })
  .always(function () {
      setOpacity("table", 1);
  });



    });



    function setOpacity(selector, opacity) {
        $(selector).css('opacity', opacity);
    }

    function DrawPriceListRow(listSelector, id, pl) {
        console.log(pl);
        var list = $(listSelector).append(
        //'<tr bgcolor="' + (pl.Comparable ? "#C1C9F5" : "#FFFFFF") + '">' +
              '<tr  style="background: ' + (pl.Comparable ? "rgb(187, 187, 187)" : "#FFFFFF") + '">' +
             '<td> <input type="radio" name="' + id + '" value=' + pl.Id + '></td>' +
             '<td>' + pl.Name + '</td>' +
             '<td>' + pl.SignDate + '</td>' +
             (pl.Comparable ?
            '<td style=" padding-top: 1px; "><button style="display:' + (pl.Approved ? 'none' : '') + '" type="button" name="approve" class="btn btn-default btn-small" value="' + pl.RevId + '" onclick="cbApprove(' + pl.RevId + ');">Утв</button><button style="display:' + (!pl.Approved ? 'none' : '') + '" type="button" name="disapprove" class="btn btn-default btn-small" value="' + pl.RevId + '" onclick="cbDisapprove(' + pl.RevId + ');">Разутв</button>'

        //'<td style=" padding-top: 1px; ">' + (pl.Approved = 'true' ?

        //'<button type="button" class="btn btn-default btn-small" value="' + pl.RevId + '" onclick="cbApprove(' + pl.RevId + ');">Утв</button>' :
        //'<button type="button" class="btn btn-primary btn-small" onclick="cbClick(' + pl.RevId + ');">Отм</button>'
        // ) 
           + '<button type="button" class="btn btn-danger btn-small" onclick="deletePL(' + +pl.RevId + ')">X</button>'
        //'<td><input type="checkbox" onclick="cbClick(' + pl.RevId + ');" value=' + '"' + +pl.RevId + '"' + (pl.Approved = 'true' ? 'checked' : '') + '>'
             : '<td style=" padding-top: 5px; ">Не требует утв.')
        // 

             + '<a href="' + downloadUrl + pl.FileId + '" class="icon-download-alt pull-right "></a></td>' +
              +'</tr>');
        // '<tr></tr>');

    }
})

function cbClick(revId, approve) {

    var cb = $('button[value="' + revId + '"]');
    console.log(cb);
    if (cb) {


        $.post(approveDisapproveUrl + '?' + Math.random(), { revId: revId, status: approve }, function (json, textStatus) {
            if (json) {

                console.log(json);
                //$(cb).attr('checked', json.status);
                var row = $(cb).closest("tr");

                if (json.success != true) {

                    if (row) {
                        $(row).css("background", "#F77B7B");
                        $(row).animate({
                            backgroundColor: "#aa0000"
                        }, 1000);
                    }
                    alert('Sorry, you have no right for this action');
                }
                else {
                    if (row) {
                        $(row).css("background", "#9FEE9F");
                        $(row).animate({
                            backgroundColor: "#aa0000"
                        }, 1000);
                        toggleBtn(revId, json.status);
                    }
                }

            }
        }, "json");



    }

    console.log(revId);
}
function cbApprove(revId) {
    cbClick(revId, true);
}

function deletePL(revId) {
    var result = confirm("Want to delete?");
    if (result == true) {
        var cb = $('button[value="' + revId + '"]');
        $.post(deletePlUrl + '?' + Math.random(), { revId: revId }, function (json) {
            {

                console.log(json);

                var row = $(cb).closest("tr");

                if (json != true) {

                    if (row) {
                        $(row).css("background", "#F77B7B");
                    }
                    alert('Sorry, you have no right for this action');
                }
                else {
                    if (row) {
                        $(row).remove();

                    }
                }

            }
        }, "json");
    }
}
function cbDisapprove(revId) {
    cbClick(revId, false);
}
function toggleBtn(revId, approved) {
    var approveBtn = $('button[name="approve"][value="' + revId + '"]');
    var disApproveBtn = $('button[name="disapprove"][value="' + revId + '"]');
    $(approveBtn).toggle(!approved);
    $(disApproveBtn).toggle(approved);

}