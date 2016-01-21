$(document).ready(function()
{
    //DynamicList();

    //PositionList = GetPositionList();
    PageLoaded()

    $('#aps').iphoneStyle();
    window.PosList = [];

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
                    url: toListUrl+'?'+Math.random(),
                    dataType: "json"
                }
            }
        }
    });

    function TOOnChange() {
        var toModel = GetTOItemList();
        
        ClearTable();
        if (toModel != null) {
            
            Init();
        }
    }


    function Init() {
        var toItemList = GetTOItemList();
        $('#Region').val(toItemList.Region); 
        $('#WOVAT').prop('checked', toItemList.WOVAT);
        $('#Brunch').val(toItemList.Brunch);
        $('#Type').val(toItemList.Type);
        $('#Subcontractor').val(toItemList.Subcontractor);
        $('#Activity').val(toItemList.Activity);
        AddHeaders(toItemList.Type);
        for (index = 0; index < toItemList.Items.length; ++index) {
            AddRow(toItemList.Type, toItemList.Items[index]);
        }
        if(toItemList.MatItems){
         for (index = 0; index < toItemList.MatItems.length; ++index) {
            AddMatRow(toItemList.Type, toItemList.MatItems[index]);
        }
        }
       // PositionList = GetPositionList();
        DynamicList();
        PositionList=null;
        toItemList=null;
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
        SetTotalPrice();
       // ClearUnbounded();

        ColoredTable();
    }

   


    //Страница загружена
    function PageLoaded() {
        $(".content-loader").fadeOut();
        $(".content-loader").removeClass("content-back-white");
    }
})

function FormOnComplete(data) {
    $("#to-message").empty();
    $("#to-message").show();
    if (data.Success) {
       // $("#to-message").append("<div class='alert alert-success'><button type='button' class='close' data-dismiss='alert'>×</button>" + data.Message + "<a href='" + data.Url + "'> link.</a></div>");
        $("#to-message").append("<div class='alert alert-success'><button type='button' class='close' data-dismiss='alert'>×</button>" + data.Message );
       // ClearTableContents();
    } else {
        $("#to-message").append("<div class='alert alert-error'><button type='button' class='close' data-dismiss='alert'>×</button>" + data.Message + "</div>");
    }


}

function Enablebtn() {
    $("#Sendbtn").prop("disabled", false);
}

function Disablebtn() {
    $("#Sendbtn").prop("disabled", true);
    $("#to-message").hide();
}



function AddHeaders(type) {

    $("#targetTable thead").append("<tr></tr>");
    switch (type) {
        //case 'АМС': {
        //      $("#targetTable thead tr").append("<th>Номер сайта</th>")
        //                                .append("<th>Адрес сайта</th>")
        //                                .append("<th>Тип</th>")
        //                                .append("<th>Кол-во</th>")
        //                                .append("<th>Код</th>")
        //                                .append("<th>Цена</th>")
        //                                .append("<th>Сумма</th>");
        //    break;
        //}
      
        
        default: {
            $("#targetTable thead tr").append("<th>Номер сайта</th>")
                                       .append("<th>Адрес сайта</th>")
                                       .append("<th>Тип</th>")
                                       .append("<th>Кол-во</th>")
                                       .append("<th>Код</th>")
                                       .append("<th>Цена</th>")
                                       .append("<th>Сумма</th>");
        }
    }
}

String.prototype.format = String.prototype.f = function () {
    var args = arguments;
    return this.replace(/\{\{|\}\}|\{(\d+)\}/g, function (m, n) {
        if (m == "{{") { return "{"; }
        if (m == "}}") { return "}"; }
        return args[n];
    });
};

function AddRow(type,row) {

    var index = $("#targetTable tbody tr").length;
    $("#targetTable tbody").append('<tr pk="{0}" class="{1}"></tr>'.f(index, 'TableRow'));
    if(row){
       
        switch (type) {
            //case 'АМС': {
            //    $("#targetTable tbody tr:last").append('<td>{0}</td>'.f(row.Site))
            //                                   .append('<td>{0}</td>'.f(row.SiteAddress))
            //                                    .append('<td name={1}  style="background: #B8D7F8;">{0}</td>'.f(row.Description,'Items[{0}].Description'.f(index) ))
            //                                    .append('<td><div name="{1}">{0}</div></td>'.f(row.SiteQuantity, 'Items[{0}].Quantity'.f(index)))
                    
            //                                    .append('<td  pk="{1}"><input required name="{0}" value="{4}"></input><input hidden name="{2}" value="{3}"></input></td>'.f('Items[{0}].ItemId'.f(index), index, 'Items[{0}].TOItem'.f(index), row.TOItem, (row.ItemId)?row.ItemId:""))
            //                                     .append('<td><div name="{1}">{0}</div><input hidden name="_{1}"></input> </td>'.f('0', 'Items[{0}].Price'.f(index)))
            //                                      .append('<td><div name="{1}">{0}</div><input hidden name="_{1}"></input></td>'.f('0', 'Items[{0}].TotalPrice'.f(index)));


                 
            //    break;
            //}
          
          // первый инпут хранит в себе ид позиции из прайслиста, а второй позицию из сайтхендлера  
            default: {
                $("#targetTable tbody tr:last").append('<td>{0}</td>'.f(row.Site))
                                              .append('<td>{0}</td>'.f(row.SiteAddress))
                                               .append('<td name={1}  style="background: #B8D7F8;">{0}</td>'.f(row.Description, 'Items[{0}].Description'.f(index)))
                                               .append('<td><div name="{1}">{0}</div></td>'.f(row.SiteQuantity, 'Items[{0}].Quantity'.f(index)))

                                               //.append('<td  pk="{1}"><input required name="{0}" value="{4}"></input><input hidden name="{2}" value="{3}" style=" display: none; "></input></td>'.f('Items[{0}].ItemId'.f(index), index, 'Items[{0}].TOItem'.f(index), row.TOItem, (row.ItemId) ? row.ItemId : ""))
                .append('<td  pk="{1}"><div class="input-prepend"> <input class="span6" name="{5}" type="text"> <a class="btn" href="#"><i class=" icon-chevron-down"></i></a>   </div><input hidden required name="{0}" value="{4}"></input><input hidden name="{2}" value="{3}" style=" display: none; "></input></td>'.f('Items[{0}].ItemId'.f(index), index, 'Items[{0}].TOItem'.f(index), row.TOItem, (row.ItemId) ? row.ItemId : "", 'Items[{0}].ItemName'.f(index)))
                .append('<td><div name="{1}">{0}</div><input hidden name="_{1}" style=" display: none;" value="{0}"></input> </td>'.f(row.Price, 'Items[{0}].Price'.f(index)))
                                                 .append('<td><div name="{1}">{0}</div><input hidden name="_{1}" style=" display: none; " value="{0}"></input></td>'.f(row.Total, 'Items[{0}].TotalPrice'.f(index)));

            }
        }
    }
}


function AddMatRow(type,row) {

    var index = $("#materialTable tbody tr").length;
    $("#materialTable tbody").append('<tr pk="{0}" class="{1}"></tr>'.f(index, 'TableRow'));
    if(row){
       
        switch (type) {
            //case 'АМС': {
            //    $("#targetTable tbody tr:last").append('<td>{0}</td>'.f(row.Site))
            //                                   .append('<td>{0}</td>'.f(row.SiteAddress))
            //                                    .append('<td name={1}  style="background: #B8D7F8;">{0}</td>'.f(row.Description,'MatItems[{0}].Description'.f(index) ))
            //                                    .append('<td><div name="{1}">{0}</div></td>'.f(row.SiteQuantity, 'MatItems[{0}].Quantity'.f(index)))
                    
            //                                    .append('<td  pk="{1}"><input required name="{0}" value="{4}"></input><input hidden name="{2}" value="{3}"></input></td>'.f('MatItems[{0}].ItemId'.f(index), index, 'MatItems[{0}].TOItem'.f(index), row.TOItem, (row.ItemId)?row.ItemId:""))
            //                                     .append('<td><div name="{1}">{0}</div><input hidden name="_{1}"></input> </td>'.f('0', 'MatItems[{0}].Price'.f(index)))
            //                                      .append('<td><div name="{1}">{0}</div><input hidden name="_{1}"></input></td>'.f('0', 'MatItems[{0}].TotalPrice'.f(index)));


                 
            //    break;
            //}
          
            
            default: {
                $("#materialTable tbody tr:last").append('<td>{0}</td>'.f(row.Site))
                                              .append('<td>{0}</td>'.f(row.SiteAddress))
                                               .append('<td name={1}  style="background: #B8D7F8;">{0}</td>'.f(row.Description, 'MatItems[{0}].Description'.f(index)))
                                               .append('<td><div name="{1}">{0}</div></td>'.f(row.Quantity, 'MatItems[{0}].Quantity'.f(index)))
                                                      .append('<td  pk="{1}">{0}</input><input hidden name="{2}" value="{3}" style=" display: none; "></input></td>'
                                                      .f(row.ECRADD?'<div>ECRADD</div>':'<input required name="{0}" value="{1}">'.f('MatItems[{0}].ItemId'.f(index),(row.ItemId) ? row.ItemId : ""),
                                                      index,
                                                      'MatItems[{0}].MatItem'.f(index),
                                                      row.MatItem
                                                      )
                                                       
                                                      )
                                              // .append('<td  pk="{1}"><input required name="{0}" value="{4}"></input><input hidden name="{2}" value="{3}" style=" display: none; "></input></td>'.f('Items[{0}].ItemId'.f(index), index, 'Items[{0}].MatItem'.f(index), row.TOItem, (row.ItemId) ? row.ItemId : ""))
                                                .append('<td><div name="{1}">{0}</div><input hidden name="_{1}" style=" display: none;" value="{0}"></input> </td>'.f(row.Price, 'MatItems[{0}].Price'.f(index)))
                                                 .append('<td><div name="{1}">{0}</div><input hidden name="_{1}" style=" display: none; " value="{0}"></input></td>'.f(row.Total, 'MatItems[{0}].TotalPrice'.f(index)));

            }
        }
    }
}
function ClearTable() {
    $("#targetTable thead tr").remove();
    $("#targetTable tbody tr").remove();
     $("#materialTable thead tr").remove();
    $("#materialTable tbody tr").remove();
         $('#Region').val('');
        $('#Brunch').val('');
        $('#Type').val('');
        $('#Subcontractor').val('');
        $('#Activity').val('');
      //  $('.k-list-container[id!="TO-list"]').remove();

}

function GenerateSelectList()
{
    var ddListHtml = "";
    ddListHtml += "<option value=0></option>";
    var posIndex = 0;
    var posLength = window.PosList.length;
    for (; posIndex < posLength; posIndex++) {
        var item = window.PosList[posIndex];
        ddListHtml += "<option value=" + item.value + ">" + item.text + "</option>";
        //if (posIndex + 1 < posLength && posIndex % 100 == 0) {
        //    setTimeout(process, 0);
        //}
    }
    return ddListHtml;
}

function DynamicList() {
    window.PosList = GetPositionList();
  //  console.log(posList);
    var elements = $("[name^='Items'][name$='ItemId']");
    var length = elements.length;
    var index = 0;

    var process = function () {
        for(;index<length;index++){
            var element = elements[index];
            //$(element).change(CodeOnChange);
            //$(element).mousedown(clickOnSelect);
            if (index + 1 < length && index % 100 == 0) {
                setTimeout(process, 0);
            }
           
            var elementName = $(element).attr("name");
            var el = document.getElementsByName(elementName)[0];
            var value = el.getAttribute("value");
            var nameElement = $(element).closest("div").find("[name^='Items'][name$='ItemName']")
            var button = $(element).closest("div").find("a");
            $(button).click(function (ev) {
                ev.preventDefault();
                $("#myModal").modal();
            });
            var posValues = jQuery.grep(window.PosList, function (obj) {
                return obj.value == value;
            })
            var posValue = posValues[0];
           // $(element).html(posValue.text);
            $(nameElement).val(posValue.text);
            element = null;
               
        }
    };
    process();
    soList = null;
    elements = null;


        //.each(function (i) {
        //var self = this;
        //$.each(posList, function (po,item) {
        //   // console.log(item);
        //    $(self).append("<option value="+item.value+">"+item.text+"</option>");
        //});
    //});
    //$("[name^='Items'][name$='ItemId']").combobox({
    //    bsVersion: '2',
       
    //}).change(CodeOnChange);

    //$("[name^='Items'][name$='ItemId']").width(340).kendoComboBox({
    //    dataTextField: "text",
    //    dataValueField: "value",
    //    placeholder: "Select...",
    //    dataSource: PositionList,
    //    filter: "contains",
    //    height: 500,

    //   // dataBound:onDataBound,
    //    change: CodeOnChange
    //});
    // $("[name^='MatItems'][name$='ItemId']").width(340).kendoComboBox({
    //    dataTextField: "text",
    //    dataValueField: "value",
    //    placeholder: "Select...",
    //    dataSource: PositionList,
    //    filter: "contains",
    //    height: 500,
      
    //   // dataBound:onDataBound,
    //    change: MatCodeOnChange
    //});
       
}

function clickOnSelect(ev)
{
    if($(ev.currentTarget).prop("class").indexOf("init")==-1)
    {
        //var elementName = $(ev.currentTarget).attr("name");
        //var el = document.getElementsByName(elementName)[0];
        var value = ev.currentTarget.getAttribute("value");

        var selList = GenerateSelectList();
        $(this).html('');
        $(this).append(selList);
        $(this).addClass("init");
        $(this).val(value);
    }
}

function NumParse(text) {
    return parseFloat(text.replace(',', '.'), 10);
}

// function onDataBound(e) {
//                       
//                       if(e.sender.select()==-1)
//                       {
//                        e.sender.value("");
//                        }
//                      
//                    };
function ClearUnbounded()
{
       $("[name^='Items'][name$='ItemId']").each(function(index,item){
        var kombo = item.data('kendoComboBox');
        if(kombo.select()==-1)
                       {
                       komboo.value("");
                        }
        });
    
       
}

//Обновление списка позиций при изменении параметров
function GetPositionList() {

   
    var calb = null;
    var d = { ToId: $('#TO').val() };
    $.ajax({
        type: "POST",
        url: toPositionUrl + '?' + Math.random(),
        contentType: "application/json",
        async: false,
        data: JSON.stringify(d),
        success: function (response) {
            calb = eval(response);
        }
    });
    return calb;
}

function FormatDecimal(dec) {
    if (dec) {
        return parseFloat(dec.toFixed(2));
    }
    else
        return 0;
}
//Определение имени поля
//Items зашито в конструкторе Js файла jquery.mvc-template-row.js
function GetFieldName(pk) {

    // Items\\[0\\]
    return 'Items\\[{0}\\]'.f(Math.abs(pk));
}
//Событие изменения кода позиции в конкретной строке
function CodeOnChange() {
    //if (this.value() && this.selectedIndex == -1) {
    //    // var dt = this.dataSource._data[0];                
    //    this.text('');
    //    this._selectItem();
    //}
    var key = $(this).closest('td').attr('pk');
    console.log(key);
    UpdatePrices(key);
    AutoFillKendoComboBoxTOv2(key);
    ColoredTable();
}

function MatCodeOnChange() {
    if (this.value() && this.selectedIndex == -1) {
        // var dt = this.dataSource._data[0];                
        this.text('');
        this._selectItem();
    }
    var key = $(this.element).closest('td').attr('pk');
    console.log(key);
    UpdateMatPrices(key);
    AutoFillKendoComboBoxTO(key);
    ColoredTable();
}

function UpdatePrices(key, price) {
    var Item = $('select[name="Items[' + parseInt(key) + '].ItemId"]:first');
    if (Item) {
        var quantityDiv = $('div[name^=Items\\[{0}\\]][name$=Quantity]'.f(key));
        var priceDiv = $('div[name^=Items\\[{0}\\]][name$=Price]'.f(key));
        var _priceDiv = $('input[name^=_Items\\[{0}\\]][name$=Price]'.f(key));;
        var totalPriceDiv = $('div[name^=Items\\[{0}\\]][name$=TotalPrice]'.f(key));
        var _totalPriceDiv = $('input[name^=_Items\\[{0}\\]][name$=TotalPrice]'.f(key));

        var itemId = Item.val();
       
        var quantity = $(quantityDiv).html();
        if (itemId) {
            if (typeof price == 'undefined')
                price = GetPriceListItemPrice(itemId);

        }
        $(_priceDiv).val(price);
        $(priceDiv).html(FormatDecimal(price));
        $(_totalPriceDiv).val(price * quantity);
        $(totalPriceDiv).html(FormatDecimal(price * quantity));
        SetTotalPrice();
    }
}

function UpdateMatPrices(key, price) {
    var Item = $('input[name="MatItems[' + parseInt(key) + '].ItemId"]:first').data('kendoComboBox');
    if (Item) {
        var quantityDiv = $('div[name^=MatItems\\[{0}\\]][name$=Quantity]'.f(key));
        var priceDiv = $('div[name^=MatItems\\[{0}\\]][name$=Price]'.f(key));
        var _priceDiv = $('input[name^=_MatItems\\[{0}\\]][name$=Price]'.f(key));;
        var totalPriceDiv = $('div[name^=MatItems\\[{0}\\]][name$=TotalPrice]'.f(key));
        var _totalPriceDiv = $('input[name^=_MatItems\\[{0}\\]][name$=TotalPrice]'.f(key));

        var itemId = Item.value();
       
        var quantity = $(quantityDiv).html();
        if (itemId) {
            if (typeof price == 'undefined')
                price = GetPriceListItemPrice(itemId);

        }
        $(_priceDiv).val(price);
        $(priceDiv).html(FormatDecimal(price));
        $(_totalPriceDiv).val(price * quantity);
        $(totalPriceDiv).html(FormatDecimal(price * quantity));
        SetTotalPrice();
    }
}

//Получение данных о ПОРе
function GetTOItemList() {
    var calb = null;
    var d = { ToId: $('#TO').val() };
    $.ajax({
        type: "POST",
        url: toDataUrl + '?' + Math.random(),
        contentType: "application/json",
        async: false,
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
                calb = response;
            }

        }
    });

    return calb;
}

function GetTotalPrice() {
    var totals = $('input[name^=_Items][name$=TotalPrice],[name^=_MatItems][name$=TotalPrice]');
    var sum = parseFloat(0);
    for (index = 0; index < totals.length; ++index) {
        if ($(totals[index]).val())
        {
            sum = sum + parseFloat($(totals[index]).val());
        }
    }
    return sum;
}

function SetTotalPrice() {
    var total = GetTotalPrice();
    $('#TotalPrice').val(FormatDecimal(total));
}

function GetPriceListItemPrice(id) {
    var calb = null;
    $.ajax({
        url: priceListUrl + '?' + Math.random(),
        data: { id: id },
        async: false,
        type: "POST",
        // указываем URL и
        //dataType : "string",                     // тип загружаемых данных
         success: function (data) { // вешаем свой обработчик на функцию success
             calb = eval(data);
         },
        error: function (request, status, error) {
        alert(error);
        }
    });
    return calb;
    

}

function ColoredTable() {
    var emptyInputs = $('input[name$=ItemId][value=]');
    
    var table = $("#targetTable");
    if (emptyInputs.length!=0) {
        $(table).css("background", '');

    }
    else {
        $(table).css("background", '#DAF8DD');
    }
    emptyInputs=null;
}
