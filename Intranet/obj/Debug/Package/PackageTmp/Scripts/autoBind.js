$(function () {
    $('#autoMap').live("click", function () { autoMap(); });
 })


// если стоит чекбокс, то заполняет автоматически похожие поля
function AutoFillKendoComboBox(key) {
    var val = $("[id$='" + key + "__PositionId']:first").data('kendoComboBox').value();
    var aps = $('#aps').prop('checked');
    if (aps) {

        // находим дискрипшн рядом  с измененным полем
        var descriptionField = $("[name*='[" + (parseInt(key)) + "].SHDescription']:first")
        // console.log(descriptionField.val());

        // находим все похожие дискрипшены
        var sameDescriptionFields = $("[name*='SHDescription']:contains('" + descriptionField.val() + "')").not(descriptionField);
        // console.log(sameDescriptionFields);
        // находим для каждой позишн ади
        sameDescriptionFields.each(function () {
            var numberPattern = /\d+/g;

            var key2 = $(this).attr('id').match(numberPattern);
            // console.log(key2);
            $("[id$='" + key2 + "__PositionId']:first").data('kendoComboBox').value(val);

            InsertPrices(key2);
            DrawFields(key2);
            //$("[name*='[" + parseInt(key2) + "].PositionId']:first").data("kendoComboBox").value(val);

        })
    }
}

function AutoFillKendoComboBoxTO(key) {
    var val = $('input[name="Items['+parseInt(key)+'].ItemId"]:first').data('kendoComboBox').value();;
    if (val) {
        var aps = $('#aps').prop('checked');
        if (aps) {
            // находим дискрипшн рядом  с измененным полем
            var descriptionField = $('td[name="Items[' + parseInt(key) + '].Description"]:first');
            //console.log(descriptionField);
            var description = descriptionField.html();
            var price = GetPriceListItemPrice(val);
            // находим все похожие дискрипшены
            var sameDescriptionFields = $('td[name$="Description"]:contains("' + description + '")').not(descriptionField);
           // console.log(sameDescriptionFields);
            // находим для каждой позишн ади
            sameDescriptionFields.each(function () {
                var numberPattern = /\d+/g;
               
                var key2 = $(this).attr('name').match(numberPattern);
              //   console.log(key2);
                var key2DDBox = $('input[name="Items[' + parseInt(key2) + '].ItemId"]:first').data('kendoComboBox');
                key2DDBox.value(val);
                UpdatePrices(key2,price);

              //  InsertPrices(key2);
              //  DrawFields(key2);
                //$("[name*='[" + parseInt(key2) + "].PositionId']:first").data("kendoComboBox").value(val);
            });

            }
    }
}

function AutoFillKendoComboBoxTOv2(key) {
    var val = $('select[name="Items[' + parseInt(key) + '].ItemId"]:first').val();
    if (val) {
        var aps = $('#aps').prop('checked');
        if (aps) {

            var posValues = jQuery.grep(window.PosList, function (obj) {
                return obj.value == val;
            })
            var posValue = posValues[0];


            // находим дискрипшн рядом  с измененным полем
            var descriptionField = $('td[name="Items[' + parseInt(key) + '].Description"]:first');
            //console.log(descriptionField);
            var description = descriptionField.html();
            var price = GetPriceListItemPrice(val);
            // находим все похожие дискрипшены
            var sameDescriptionFields = $('td[name$="Description"]:contains("' + description + '")').not(descriptionField);
            // console.log(sameDescriptionFields);
            // находим для каждой позишн ади
            var length = sameDescriptionFields.length;
            var index = 0;
            var process = function () {
                for(;index<4;index++)
                {
                    var el = sameDescriptionFields[index];
                   
                    var numberPattern = /\d+/g;

                    var key2 = $(el).attr('name').match(numberPattern);
                    //   console.log(key2);
                    var key2DDBox = $('select[name="Items[' + parseInt(key2) + '].ItemId"]:first');
                   
                   // $(key2DDBox).html('');
                    $(key2DDBox).append("<option value=" + posValue.value + ">" + posValue.text + "</option>");
                    $(key2DDBox).val(posValue.value);
                   // alert(posValue.value);
                    UpdatePrices(key2, price);

                    if (index + 1 < length && index % 100 == 0) {
                        setTimeout(process, 0);
                    }
                }
            }
            process();

            //sameDescriptionFields.each(function () {
               

            //    //  InsertPrices(key2);
            //    //  DrawFields(key2);
            //    //$("[name*='[" + parseInt(key2) + "].PositionId']:first").data("kendoComboBox").value(val);
            //});

        }
    }
}
   // console.log(val);
    //var aps = $('#aps').prop('checked');
    //if (aps) {

    //    // находим дискрипшн рядом  с измененным полем
    //    var descriptionField = $("[name*='[" + (parseInt(key)) + "].SHDescription']:first")
    //    // console.log(descriptionField.val());

    //    // находим все похожие дискрипшены
    //    var sameDescriptionFields = $("[name*='SHDescription']:contains('" + descriptionField.val() + "')").not(descriptionField);
    //    // console.log(sameDescriptionFields);
    //    // находим для каждой позишн ади
    //    sameDescriptionFields.each(function () {
    //        var numberPattern = /\d+/g;

    //        var key2 = $(this).attr('id').match(numberPattern);
    //        // console.log(key2);
    //        $("[id$='" + key2 + "__PositionId']:first").data('kendoComboBox').value(val);

    //        InsertPrices(key2);
    //        DrawFields(key2);
    //        //$("[name*='[" + parseInt(key2) + "].PositionId']:first").data("kendoComboBox").value(val);

    //    })
    //}
//}
function autoMapLoader() {
  $( ".content-loader" ).animate({
    opacity:"show",
  
  }, 500, function() {
     autoMap();
  });
}


function autoMap() {

    console.log("autoMap() begins");

    var trows = $('tr[pk]');
    for (var row = 0; row < trows.length; row++) {
        var tr = trows[row];
        

        var key = $(tr).attr('pk');
        var posiblyEcrAdd = $(trows[key]).find("[id$='" + key + "__PositionId'][readonly='readonly']:first");
        if (posiblyEcrAdd.length == 0) {


            var descriptionField = $("[name*='[" + (parseInt(key)) + "].SHDescription']:first")

            var descriptionText = $.trim(descriptionField.text());
          


            var UL = $('[id$="_' + row + '__PositionId_listbox"]');

            var option = $(UL).children('li:contains("' + descriptionText + '"):first');


            if (option.length > 0) {
                option.click();
            }
            else {
                // нечеткий поиск
                var opttest = $(UL).children();
                var distanceArray = [];
                var distAndElements = [];
                for (var i = 0; i < opttest.length; i++) {

                    var opttext = $(opttest[i]).text().slice(21);
                    var distance = levenshtein(descriptionText, opttext, 1, 1, 1);

                    distanceArray.push(distance);
                    var distAndEle = new Object();
                    distAndEle.Distance = distance;
                    distAndEle.Element = opttest[i];
                    distAndElements.push(distAndEle);
                }
                var minDistance = Math.min.apply(Math, distanceArray);
                console.log('min-'+minDistance);
                 console.log('max-'+descriptionText.length/3);
                if (minDistance < descriptionText.length/3) {
                    var element = distAndElements.filter(function (v) {
                        return v.Distance === minDistance;
                    })[0].Element;
                    $(element).click();
                }



            }

        }
       





    }
    $(".content-loader").hide();
}
var cuthalf = 150;
var buf = new Array((cuthalf * 2) - 1);

function min3(a, b, c) { // вспомогательная функция
    return Math.min(Math.min(a, b), c);
}

function levenshtein(s, t, cd, ci, cs) {
    var i, j, m, n, cost, flip, result;
    s = s.substr(0, cuthalf);
    t = t.substr(0, cuthalf);
    m = s.length;
    n = t.length;
    if (m === 0)
        result = n;
    else if (n === 0)
        result = m;
    else {
        cd = cd || 1;
        ci = ci || 1;
        cs = cs || 1;
        var minCost = min3(cd, ci, cs);
        var minD = Math.max(minCost, (m - n) * cd);
        var minI = Math.max(minCost, (n - m) * ci);
        flip = false;
        for (i = 0; i <= n; i++)
            buf[i] = i * minD;
        for (i = 1; i <= m; i++) {
            if (flip)
                buf[0] = i * minI;
            else
                buf[cuthalf] = i * minI;
            for (j = 1; j <= n; j++) {
                if (s.charAt(i - 1) == t.charAt(j - 1))
                    cost = 0;
                else
                    cost = cs;
                if (flip)
                    buf[j] = min3((buf[cuthalf + j] + cd), // delete
            (buf[j - 1] + ci),                   // insert
            (buf[cuthalf + j - 1] + cost));      // substitute
                else
                    buf[cuthalf + j] = min3((buf[j] + cd), // delete
            (buf[cuthalf + j - 1] + ci),         // insert
            (buf[j - 1] + cost));                // substitute
            }
            flip = !flip;
        }
        if (flip)
            result = buf[cuthalf + n];
        else
            result = buf[n];
    }
    return result;
}