
$(function () {


    kendo.culture("ru-RU");
    $("#TO").width(465).kendoComboBox({
        dataTextField: "text",
        dataValueField: "value",
        placeholder: "Select...",
        filter: "contains",
        suggest: true,
        change: TOOnChange,
        dataSource: {
            transport: {
                read: {
                    url: toListUrl + '?' + Math.random(),
                    data:{year:$("#year").val(),filter:$("#filter-cb").prop("checked")},
                    dataType: "json"
                }
            }
        }
    });
    var yearCombobox = $("#year");
    if (yearCombobox)
        yearCombobox.change(function () {
            var value = $(this).val();
            var combobox = $("input[name='TO']").data("kendoComboBox");
            combobox.dataSource.read({ year: value });
            combobox.text("");
            ClearTable();
        })
   
    PageLoaded();
    $("#set-dates").click(function () {
        setDates();
    });

    $('#filter-bt').click(function () {
        var cb = $('#filter-cb');
        var result = cb.prop("checked");
        $(cb).attr("checked", !result);
        if (!result) {
            $(this).addClass("pushed-button");
            $(cb).prop("checked", "checked");
        }
        else {
            $(this).removeClass("pushed-button");
            $(cb).prop("checked", false);
        }
        refreshToList();
        TOOnChange();
    });

})

function refreshToList()
{
    

        var newdataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: toListUrl + '?' + Math.random(),
                    data: { year: $("#year").val(), filter: $("#filter-cb").prop("checked") },
                    dataType: "json"
                }
            }
        });

        var ddl = $('#TO').data("kendoComboBox");
        ddl.setDataSource(newdataSource);
        //ddl.dataSource.read();
       // ddl.refresh();

    
}

function Disablebtn() {
    $("#Sendbtn").prop("disabled", true);
    $("#to-message").hide();
}
//Страница загружена
function PageLoaded() {
    $(".content-loader").fadeOut();
    $(".content-loader").removeClass("content-back-white");
    $("#StartDate").kendoDatePicker({
     
        format: "dd.MM.yyyy"
    });
    $("#EndDate").kendoDatePicker({

        format: "dd.MM.yyyy"
    });



}

function TOOnChange() {
    var dfd = $.Deferred();
    ClearTable().done(
        GetTOItemList().done(
            function (response) {
                $(".k-combobox>span .k-icon").addClass("k-loading");
           
                $('#StartDate').data("kendoDatePicker").value(null);
                $('#EndDate').data("kendoDatePicker").value(null);
                if (response != null) {

                    var promise = Init(response);
                    promise.then(function () { $(".k-combobox>span .k-icon").removeClass("k-loading"); });
                    dfd.resolve();

                }
           
            }

            )
     );
    return dfd.promise();

   
}


function Init(items) {
    var dfd = $.Deferred();
    setTimeout(function () {
        var toItemList
        if (items == null) {
            toItemList = GetTOItemList();
        }
        else
            toItemList = items;
        $('#region').text(toItemList.info.region);
        $('#wat').text(toItemList.info.nds);
        $('#branch').text(toItemList.info.branch);
        $('#subcontractor').text(toItemList.info.subcontractor);
        $('#po').text(toItemList.info.po);
        $('#poDate').text(toItemList.info.poDate);
        var showAct = !$("#filter-cb").prop('checked');
        AddHeaders(showAct);
        AddMatHeaders();
        
        if (toItemList.Items) {

            for (index = 0; index < toItemList.Items.length; ++index) {
                AddRow(toItemList.Items[index],showAct);
            }
        }
        if (toItemList.Materials) {
            for (index = 0; index < toItemList.Materials.length; ++index) {
                AddMatRow(toItemList.Materials[index]);
            }
        }


        PositionList = null;
        toItemList = null;
        //DrawFields();
        // Stripes();
        //$(':checkbox').iphoneStyle();
        //$("#PriceTotal").width(110).kendoNumericTextBox({
        //    decimals: 2,
        //    spinners: false
        //});
        ////дисаблим окошко с полной ценой
        ////        $("#TotalPrice").data("kendoNumericTextBox").enable(false);
        //$("#PriceTotalSH").width(110).kendoNumericTextBox({
        //    decimals: 2,
        //    spinners: false
        //});
        $("table").stickyTableHeaders();
        //  SetTotalPrice();
        // ClearUnbounded();

        // ColoredTable();
        $("[name$='Quantity']").kendoNumericTextBox({
            min: 0,
            format: "# шт."

        });

        $('input[type="checkbox"][name^="Services"]').click(function () {
            $('#toggle').prop('checked', false);
            setCount();

        });
        $('#toggle').click(function () {
            if ($('#toggle').prop('checked')) {
                $('input[type="checkbox"][name^="Services"]:visible').prop("checked", "checked");

            }
            else {
                $('input[type="checkbox"][name^="Services"]:visible').prop("checked", false);
            }
            setCount();
        });
        setCount();
        dfd.resolve();
    }, 100);
    return dfd.promise();
        
}

    

String.prototype.format = String.prototype.f = function () {
    var args = arguments;
    return this.replace(/\{\{|\}\}|\{(\d+)\}/g, function (m, n) {
        if (m == "{{") { return "{"; }
        if (m == "}}") { return "}"; }
        return args[n];
    });
};
Number.prototype.round = function (p) {
    p = p || 10;
    return parseFloat(this.toFixed(p));
};

String.prototype.stringToDate = function(_format, _delimiter) {
    var formatLowerCase = _format.toLowerCase();
    var formatItems = formatLowerCase.split(_delimiter);
    var dateItems = this.split(_delimiter);
    var monthIndex = formatItems.indexOf("mm");
    var dayIndex = formatItems.indexOf("dd");
    var yearIndex = formatItems.indexOf("yyyy");
    var month = parseInt(dateItems[monthIndex]);
    month -= 1;
    var formatedDate = new Date(dateItems[yearIndex], month, dateItems[dayIndex]);
    return formatedDate;
}
function setDates()
{
    var checkedBoxesCount = $('input[type="checkbox"][name^="Services"]:checked');
    var mindate = maxdate = 0;
    var init = false;
    checkedBoxesCount.each(function () {
        var factdatetext = $(this).closest("tr").children(".fact-date").html();
        var factDate = factdatetext.stringToDate("dd.MM.yyyy", '.');
        if (!init) {
            mindate = factDate;
            maxdate = factDate;
            init = true;
        }
        if (mindate > factDate)
            mindate = factDate;
        if (maxdate < factDate)
            maxdate = factDate;
    });
    $("#StartDate").data("kendoDatePicker").value(mindate);
    $("#EndDate").data("kendoDatePicker").value(maxdate);
    }
function setCount()
{
    var checkBoxesCount = $('input[type="checkbox"][name^="Services"]:visible');
    var checkedBoxesCount = $('input[type="checkbox"][name^="Services"]:checked:visible');
    if (checkBoxesCount.length == checkedBoxesCount.length)
        $('#toggle').prop('checked', "checked");
    checkBoxesCount.each(function () {
        $(this).closest("tr").removeClass("selected-row");
    });
    checkedBoxesCount.each(function () {
        $(this).closest("tr").addClass("selected-row");
    });
    var summ = 0.0;
   
    checkedBoxesCount.each(function () {
        var price = $(this).closest("tr").children(".price").html();
        var quantity = $(this).closest("tr").children(".quantity").html();
        summ = (summ+ parseFloat(price)*parseFloat(quantity)).round(2);
    });
    $("#checkCount").html("("+checkedBoxesCount.length + " из " + checkBoxesCount.length + ")");
    $("#summPrice").html("Price ("+summ.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,')+" р.)");
}
function AddRow(row,showAct) {

    var index = $("#targetTable tbody tr").length;
    $("#targetTable tbody").append('<tr pk="{0}" class="{1}"></tr>'.f(index, 'TableRow'));
    if(row){
       
       
          
            
          
        $("#targetTable tbody tr:last")
                                      .append('<td>{0}</td>'.f(row.site))
                                     .append('<td class = "fact-date">{0}</td>'.f(row.fact))
                                      .append('<td class="price">{0}</td>'.f(row.price))
                                      .append('<td>{0}</td>'.f(row.description))
                                      .append('<td class="quantity">{0}</td>'.f(row.partial?
                                                            '<input value={0} name="{1}">'.f(row.quantity, 'Services[{0}].Quantity'.f(index))
                                                            :row.quantity))
                                     .append('{0}'.f(showAct?'<td>{0}</td>'.f(row.act):''))
                                      .append('<td>{0}{1}{2}</td>'.f((
            '<div style="margin-bottom:10px"><input type="checkbox"  class="extraLarge" value="true" name="{0}"></div>').f('Services[{0}].Checked'.f(index)),
            ('<input type="hidden" value="false" name="{0}">').f('Services[{0}].Checked'.f(index)),
            '<input  name="{0}" value="{1}" style="display:none">').f('Services[{0}].Id'.f(index), row.id))
        ;

                                           

            
    }
    
}
function FormOnComplete(data) {
    $("#to-message").empty();
    $("#to-message").show();
    if (data.Success) {
        // $("#to-message").append("<div class='alert alert-success'><button type='button' class='close' data-dismiss='alert'>×</button>" + data.Message + "<a href='" + data.Url + "'> link.</a></div>");
        var url;
        if (data.Url)
            if (!data.UrlText)
                data.UrlText = "Link";
            url = "<a href='" + data.Url + "'>"+data.UrlText+"</a>"
        $("#to-message").append("<div class='alert alert-success'><button type='button' class='close' data-dismiss='alert'>×</button>" + data.Message+" "+url);
        // ClearTableContents();
    } else {
        $("#to-message").append("<div class='alert alert-error'><button type='button' class='close' data-dismiss='alert'>×</button>" + data.Message + "</div>");
    }


}

function AddMatRow(row) {

    var index = $("#materialTable tbody tr").length;
    $("#materialTable tbody").append('<tr pk="{0}" class="{1}"></tr>'.f(index, 'TableRow'));
    if(row){
       
       
          
            
          
        $("#materialTable tbody tr:last")
                                        .append('<td>{0}</td>'.f(row.site))
                                        .append('<td style="text-align:right">{0}</td>'.f(row.price))
                                        .append('<td>{0}</td>'.f(row.description))
                                        .append('<td>{0}</td>'.f(row.quantity))
                                         .append('<td>{0}{1}</td>'.f(('<input  name="{0}">').f('Materials[{0}].Quantity'.f(index)),'<input  name="{0}" value="{1}" style="display:none">').f('Materials[{0}].Id'.f(index),row.id))


            
        
    }
}

function AddHeaders(showAct) {

    $("#targetTable thead").append("<tr></tr>");
  

        
    $("#targetTable thead tr")
                           .append("<th class='span3'>Site</th>")
                           .append("<th class='span2' >Fact date <input id='date-filter'  class='valid' style='width: 80%;margin-bottom: 5px;'><button type='button' id='clear-factdate' style='margin-bottom: 5px;'>x</button></th>")
                           .append("<th class='span2'id='summPrice'>Price</th>")
                           .append("<th class='span6'>Description</th>")
                            .append("<th class='span2'>Quantity</th>")
                            .append(showAct?"<th class='span2'>Act</th>":"")
                           .append("<th style='width:10px'><div style='margin-bottom:10px'><input type='checkbox'  class='extraLarge' id='toggle'></input></div> </th>")
    ;
            
    $('#date-filter').keyup(function () { filterFactDate() });
    $('#clear-factdate').click(function () { $('#date-filter').val(""); $('#date-filter').keyup() });
}

function filterFactDate()
{
    var value = $('#date-filter').val();
    $('#toggle').prop('checked', false);
      
    if (!value)
        $('td.fact-date').parent().show();
    else {
        $('td.fact-date:contains(' + value + ')').parent().show();
        $('td.fact-date:not(:contains(' + value + '))').parent().not('td input[name$="Checked"]:checked').hide()
        $('td.fact-date').parent().has('td input[name$="Checked"]:checked').show();
    }
    setCount();
}

function AddMatHeaders() {

    $("#materialTable thead").append("<tr></tr>");
  

        
    $("#materialTable thead tr")
                           .append("<th>Site</th>")
                                       
                           .append("<th>Price</th>")
                           .append("<th>Description</th>")
                           .append("<th>SHQuantity</th>")
                           .append("<th>Quantity</th>")
    ;
            
    
}

String.prototype.format = String.prototype.f = function () {
    var args = arguments;
    return this.replace(/\{\{|\}\}|\{(\d+)\}/g, function (m, n) {
        if (m == "{{") { return "{"; }
        if (m == "}}") { return "}"; }
        return args[n];
    });
};




function ClearTable() {
    dfd = $.Deferred();
    setTimeout(function () {
        $("#targetTable thead tr").remove();
        $("#targetTable tbody tr").remove();
        $("#materialTable thead tr").remove();
        $("#materialTable tbody tr").remove();
        dfd.resolve();
    }, 100);
    return dfd.promise();
    
    //    $('#Region').val('');
    //    $('#Brunch').val('');
    //    $('#Type').val('');
    //    $('#Subcontractor').val('');
    //    $('#Activity').val('');
    //  $('.k-list-container[id!="TO-list"]').remove();

}

function NumParse(text) {
    return parseFloat(text.replace(',', '.'), 10);
}

function Enablebtn() {
    $("#Sendbtn").prop("disabled", false);
}
////Обновление списка позиций при изменении параметров
//function GetPositionList() {


//    var calb = null;
//    var d = { ToId: $('#TO').val() };
//    $.ajax({
//        type: "POST",
//        url: actPositionUrl + '?' + Math.random(),
//        contentType: "application/json",
//        async: false,
//        data: JSON.stringify(d),
//        success: function (response) {
//            calb = eval(response);
//        }
//    });
//    return calb;
//}


function GetTOItemList() {
    $(".k-combobox>span .k-icon").addClass("k-loading");
    var dfd = $.Deferred();

   // var calb = null;
    var d = { TO: $('#TO').val(), filter: $('#filter-cb').prop('checked') };
    $.ajax({
        type: "POST",
        url: actPositionUrl + '?' + Math.random(),
        contentType: "application/json",
       // async: false,
        data: JSON.stringify(d),
        success: function (response) {
            if (response.Status == "error") {
                noty({
                    text: response.Message,
                    type: "error",
                    layout: 'topLeft',
                    //closeWith: ['hover']
                });
            } else {
                dfd.resolve(response);
            }

        }
    });

    return dfd.promise();
}