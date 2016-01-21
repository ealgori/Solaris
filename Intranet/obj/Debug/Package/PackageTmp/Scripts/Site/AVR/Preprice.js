$(function () {
    
    ko.bindingHandlers.fixedStyle = {
       
        update: function (element, valueAccessor)
        {
            var value = ko.utils.unwrapObservable(valueAccessor());
            var formatted = value.toFixed(2);
            
            ko.bindingHandlers.text.update(element, function () { return formatted; });
          
          
        }
    }

    ko.bindingHandlers.refreshable = {
        update: function (element, valueAceessor, allbindings, data, context)
        {
          // valueAccessor()();
            ko.utils.unwrapObservable(valueAceessor());
            $(element).hide().fadeIn(500);
        }
    }


    var ViewModel = function () {
        var self = this;
        this.avrList = ko.observableArray([]);
        this.avrListLoaded = ko.observable(false);
        this.avrItemsListLoaded = ko.observable(true);
        this.items = ko.observableArray([]);
        this.prices = ko.observableArray([]);
        this.pricesLoaded = ko.observable(false);
        this.selectedChoice = ko.observable();
        this.selectedWS = ko.observable();
        this.selectedWE = ko.observable();
        this.postAllowed = ko.observable(true);
        this.postMessage = ko.observable();
        this.selectedPriority = ko.observable();
        this.selectedRukOtdelaBy = ko.observable();
        this.selectedCity = ko.observable();
        //this.fadeInValue = ko.observable(false);
        //this.fadeIn = function () {
        //    self.fadeInValue(false === self.fadeInValue());
        //}
        this.selectedChoice.subscribe(function (selected) {
            
            var self = this;
            self.items([]);
            self.pricesLoaded(false);
            $.when(self.GetPrices()).done(function () {
                self.pricesLoaded(true);
                self.avrItemsListLoaded(false);
                $.when(self.GetItemList()).done(function () {
                   
                    var avr = ko.utils.arrayFirst(self.avrList(), function (item) {
                        return item.avr === selected;
                    }, self);
                    //self.selectedAVR(avr);
                    var dateWS = new Date(parseInt(avr.workStart.substr(6)));
                    self.selectedWS(kendo.toString(dateWS, "dd.MM.yyyy"));
                    self.selectedRukOtdelaBy(avr.rukOtdelaBy);
                    var dateWE = new Date(parseInt(avr.workEnd.substr(6)));
                    self.selectedWE(kendo.toString(dateWE, "dd.MM.yyyy"));
                    self.selectedPriority(avr.priority);
                    self.selectedCity(avr.city);
                    self.postMessage("");
                    self.avrItemsListLoaded(true);
                });
            });
           
          
        }, this);
        this.Json = ko.computed(function () {
            return this.ToJSON();
        }, this);
        this.totalVC = ko.computed(function () {

            var sum = 0;
            if (this.items()) {
                ko.utils.arrayForEach(this.items(), function (item) {
                    sum += item.vcTotal();
                });
            }
            return sum;
        }, this)
        this.totalSH = ko.computed(function () {

            var sum = 0;
            if (this.items()) {
                ko.utils.arrayForEach(this.items(), function (item) {
                    sum += item.shTotal();
                });
            }
            return sum;
        }, this);

        this.marginTotal = ko.computed(function(){
           
            if (this.totalSH() && this.totalVC())
                return (-this.totalSH() + this.totalVC()) / this.totalSH()*100;
            else
                return 0;
        },this);


    };

    var AvrItemModel = function (data) {
        this.avr = data.avr;
        this.workStart = data.workStart;
        this.workEnd = data.workEnd;
        this.rukOtdelaBy = data.rukOtdelaBy;
        this.priority = data.priority;
        this.city = data.city;
    }

    var AvrItemJsonVM = function (data) {
        this.itemId = data.id;
        this.vcPriceListRevisionItemId = data.vcPriceListRevisionItemId;
        this.vcQuantity = data.vcQuantity;
        this.vcCustomPos = data.vcCustomPos;
        this.vcDescription = data.vcDescription;
        this.vcCustomPrice = data.vcCustomPrice;
        this.vcUseCoeff = data.vcUseCoeff;
        this.vcCoeff = data.vcCoeff;
        this.noteVC = data.noteVC;
    }

    var PriceListModel = function (data) {
        this.text = data.text;
        //this.price = ko.observable();
        this.value = data.value;
        this.price = data.price;

    }
    var ItemModel = function (data, parent) {
        this.id = data.id;
        this.noteVC = data.noteVC;
        this.shDesc = data.shDesc;
        this.shPrice = data.shPrice;
        this.shQuantity = data.shQuantity;
        this.shTotal = ko.computed(function () {
            return this.shPrice * this.shQuantity;
        }, this);
        this.vcPriceListRevisionItemId = ko.observable(data.vcPriceListRevisionItemId);
        this.vcCustomPos = ko.observable(data.vcCustomPos);
        this.vcDescription = ko.observable(data.vcDescription);
        this.vcQuantity = ko.observable(data.vcQuantity);
        this.vcCustomPrice = ko.observable(data.vcPrice);
        this.vcCoeff = 1.4;
        this.vcUseCoeff = ko.observable(data.vcUseCoeff);
        this.vcPrice = ko.computed(function () {
            var self = this;
            
            if (this.vcPriceListRevisionItemId()) {
                var founded = ko.utils.arrayFirst(parent.prices(), function (item) {
                    

                    return item.value == self.vcPriceListRevisionItemId();
                   
                });
                if (founded) {
                    if (this.vcUseCoeff())
                    {
                        return founded.price * this.vcCoeff;
                    }
                    else
                    {
                        return founded.price;
                    }
                   
                }
                else
                    return 0;
            }
            else
                return 0;

        }, this);
      
        this.parent = parent;
        this.vcTotal = ko.computed(function () {
            if (this.vcCustomPos()) {
                if (this.vcCustomPrice() && this.vcQuantity())
                    return this.vcCustomPrice() * this.vcQuantity();
                else
                    return 0;
            }
            else {
                if (this.vcPrice() && this.vcQuantity())
                    return this.vcPrice() * this.vcQuantity();
                else
                    return 0;
            }
        }, this);
        this.margin = ko.computed(function () {
            if (this.vcTotal() && this.shTotal())
                return (-this.shTotal()+this.vcTotal())/this.shTotal()*100;
            else
                return 0;
        }, this);

      




    }
            


    ViewModel.prototype.GetAVRList = function () {
        var self = this;
        $.getJSON(getAVRListUrl, function (data) {

            self.avrList(ko.utils.arrayMap(data, function (item) {
                return new AvrItemModel(item);
            }));
            self.avrListLoaded(true);
        });

    }
    ViewModel.prototype.GetItemList = function () {
        var self = this;
        var deferred = $.Deferred()
        $.getJSON(getAVRItems, {
            avrId: self.selectedChoice()
           
        }, function (data) {

            //self.items(ko.utils.arrayMap(data, function (item) {
            //    return new ItemModel(item, self);
            //}));

            var items = [];
            for (var i = 0; i < data.length; i++) {
                var dataItem = data[i];
                items[i] = new ItemModel(dataItem, self);
            }
            self.items(items);

            deferred.resolve();
        })
        return deferred.promise();
    }
    ViewModel.prototype.GetPrices = function () {
        var self = this;
        var deferred = $.Deferred();
        //   var data = [{ text: "ECR-SOLA-SER-02366 - Монтаж антенного фидера БС 7/8", value: 26739, price:12.223 },
        //   { text: "ECR-SOLA-SER-02367 - Монтаж антенного фидера БС 1+1/4", value: "26740",price:345.7 },
        //{ text: "ECR-SOLA-SER-02368 - Монтаж антенного фидера БС 1/2", value: "26741",price:678.2 },
        //{ text: "ECR-SOLA-SER-02369 - Замена оптической сборки 3G (BBU-RRU)", value: 26742, price:987 },
        //{ text: "ECR-SOLA-SER-02370 - Демонтаж антенного фидера БС (без сохранения качества кабеля)", value: 12345, price:123.2 },
        //{ text: "ECR-SOLA-SER-02371 - Демонтаж/монтаж опоры панельной антенны", value: 26744, price:1.2 },
        //{ text: "ECR-SOLA-SER-06505 - Дефектация панельной антенны, фидера с измерением КСВ", value: 26745,price:345.7 },
        //{ text: "ECR-SOLA-SER-02373 - Замена панельной антенны", value: 26746 }];
        //   self.prices(data);
        //   self.pricesLoaded(true)
        //console.log(self.selectedWS());
        //console.log(self.selectedWE());

        $.post(getPriceLists, { ProjectId: "4", SubcId: "230",nsc:"True", WorkEnd: self.selectedWS(), WorkStart: self.selectedWE() }, function (data, textStatus) {
            self.prices(ko.utils.arrayMap(data, function (item) {
                return new PriceListModel(item);
            }));
            deferred.resolve();



        }, "json");
        return deferred.promise();


    }


    ViewModel.prototype.ToJSON = function () {
        var json = new function () { };
        json.avrId = this.selectedChoice;
        json.items = [];
        ko.utils.arrayForEach(this.items(), function (item) {
            var itemVM = new AvrItemJsonVM(item);
            json.items.push(itemVM);
        })
        return (ko.toJSON(json));

    }

    ViewModel.prototype.PostJSON = function () {
        //           
        var self = this;
        //        $.post("/AVR/PostPreprice", {model: self.Json()}, function (data, textStatus) {
        //         
        //        }, "json");
        self.postAllowed(false);
        $.ajax({
            url: postUrl,
            type: 'POST',
            dataType: 'json',
            data: self.Json(),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                self.postAllowed(true);
                self.avrList.remove(function (item) { return item.avr === self.selectedChoice });
                // get the result and do some magic with it
                self.postMessage("Заказ успешно опрайсован.");
            },
            error:function(data)
            {
                self.postMessage("Ошибка при опрайсовке")
            }

        });

    }


    var vm = new ViewModel();
    ko.applyBindings(vm);
    vm.GetAVRList();

    //var avrData = $.getJSON("/Json/GetAVRList", function (data) {
    //    ko.applyBindings(new ViewModel(data));
    //})

    //ko.applyBindings(new ViewModel());

})