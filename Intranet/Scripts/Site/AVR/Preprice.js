$(function () {
    
    ko.bindingHandlers.fixedStyle = {
       
        update: function (element, valueAccessor)
        {
            var value = ko.utils.unwrapObservable(valueAccessor());
            if (!value) value = 0;
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
        this.avrItemsListLoaded = ko.observable();
        this.items = ko.observableArray([]);
        this.prices = ko.observableArray([]);
        //this.pricesLoaded = ko.observable(false);
        this.selectedChoice = ko.observable();
        this.selectedWS = ko.observable();
        this.selectedWE = ko.observable();
        this.postAllowed = ko.observable(true);
        this.postMessage = ko.observable();
        this.selectedPriority = ko.observable();
        this.selectedRukOtdelaBy = ko.observable();
        this.selectedCity = ko.observable();
        this.selectedTotal = ko.observable(0);
        //this.fadeInValue = ko.observable(false);
        //this.fadeIn = function () {
        //    self.fadeInValue(false === self.fadeInValue());
        //}
        this.selectedChoice.subscribe(function (selected) {
           
            var self = this;
            self.avrItemsListLoaded(false);
            self.clearSelected();
            self.items([]);
            self.GetPrices()
                .then(self.GetItemList())
                .then(function () {
                    var avr = ko.utils.arrayFirst(self.avrList(), function (item) {
                                    return item.avr === selected;
                                }, self);
                                
                                var dateWS = new Date(parseInt(avr.workStart.substr(6)));
                                self.selectedWS(kendo.toString(dateWS, "dd.MM.yyyy"));
                                self.selectedRukOtdelaBy(avr.rukOtdelaBy);
                                var dateWE = new Date(parseInt(avr.workEnd.substr(6)));
                                self.selectedWE(kendo.toString(dateWE, "dd.MM.yyyy"));
                                self.selectedPriority(avr.priority);
                                self.selectedCity(avr.city);
                                self.selectedTotal(avr.total);
                                self.postMessage("");
                                self.avrItemsListLoaded(true);
                                 $("#item-table").resizableColumns();

                });
        }, this);
        this.clearSelected = function () {
            this.selectedPriority("");
            this.selectedRukOtdelaBy("");
            this.selectedCity("");
            this.selectedTotal(0);
            this.selectedWS("");
            this.selectedWE("");
        };
        //this.Json = ko.computed(function () {
        //    return this.ToJSON();
        //}, this);
        this.totalVC = ko.computed(function () {

            var sum = 0;
            if (this.items()) {
                ko.utils.arrayForEach(this.items(), function (item) {
                    sum += item.vcTotal();
                });
            }
            return sum;
        }, this)
        // расчет больше не требуется
        //this.totalSH = ko.computed(function () {

        //    var sum = 0;
        //    if (this.items()) {
        //        ko.utils.arrayForEach(this.items(), function (item) {
        //            sum += item.shTotal();
        //        });
        //    }
        //    return sum;
        //}, this);

        this.marginTotal = ko.computed(function(){
           
            if (this.selectedTotal() && this.totalVC())
                return (this.totalVC() - this.selectedTotal()) / this.selectedTotal() * 100;
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
        this.total = data.total

    }

    var AvrItemJsonVM = function (data) {
        this.id = data.id;
        this.avrItemId = data.avrItemId;
        this.vcCustomPos = data.vcCustomPos();
        if (!this.vcCustomPos)
            this.priceListRevisionItemId = data.priceListRevisionItemId();
        else
            this.price = data.vcCustomPrice();
        this.quantity = data.quantity();
       
        this.description = data.description();
        
        this.vcUseCoeff = data.vcUseCoeff();
        this.noteVC = data.noteVC();
        this.workReason = data.workReason();
        this.shDescription = data.shDescription;
    }

    var PriceListModel = function (data) {
        this.text = data.text;
        //this.price = ko.observable();
        this.value = data.value;
        this.price = data.price;

    }
    var ItemModel = function (data, parent) {
        this.id = data.id;
        this.avrItemId = data.avrItemId;
        this.noteVC = ko.observable(data.noteVC);
        this.workReason = ko.observable(data.workReason);
        this.shDescription = data.shDescription;
        this.shPrice = data.shPrice;
        this.shQuantity = data.shQuantity;
        this.shTotal = ko.computed(function () {
            if (this.shPrice && this.shQuantity) {
                return this.shPrice * this.shQuantity;
            }
            else {
                return 0;
            }

        }, this);
        this.priceListRevisionItemId = ko.observable(data.priceListRevisionItemId);
        this.vcCustomPos = ko.observable(data.vcCustomPos);
        this.description = ko.observable(data.description);
        this.quantity = ko.observable(data.quantity);
        this.vcCustomPrice = ko.observable(data.price);
        this.vcCoeff = 1.4;
        this.vcUseCoeff = ko.observable(data.vcUseCoeff);
        this.price = ko.computed(function () {
            var self = this;
            
            if (this.vcCustomPos()) {
                return this.vcCustomPrice();
            }
            else {
                if (this.priceListRevisionItemId()) {
                    var founded = ko.utils.arrayFirst(parent.prices(), function (item) {


                        return item.value == self.priceListRevisionItemId();

                    });
                    if (founded) {
                        if (this.vcUseCoeff()) {
                            return founded.price * this.vcCoeff;
                        }
                        else {
                            return founded.price;
                        }

                    }
                    else
                        return 0;
                }
                else
                    return 0;
            }

        }, this);
      
        this.parent = parent;
        this.vcTotal = ko.computed(function () {
            if (this.vcCustomPos()) {
                if (this.vcCustomPrice && this.quantity())
                    return this.vcCustomPrice() * this.quantity();
                else
                    return 0;
            }
            else {
                if (this.price() && this.quantity())
                    return this.price() * this.quantity();
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

        this.Remove = function () {
            this.parent.items.remove(this);
        }

      




    }
            


    ViewModel.prototype.GetAVRList = function () {
        var self = this;
        return new Promise(function (resolve, reject) {
            $.getJSON(getAVRListUrl, function (data) {

                self.avrList(ko.utils.arrayMap(data, function (item) {
                    return new AvrItemModel(item);
                }));
                
                resolve();
            });
        });
       

    }
    ViewModel.prototype.GetItemList = function () {


        var self = this;
        return new Promise(function (resolve,reject) {
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

                resolve();
            })
        })
      
    }
    ViewModel.prototype.GetPrices = function () {
        var self = this;
        return new Promise(function (resolve, reject) {
            $.post(getPriceLists, { ProjectId: "4", SubcId: "230", nsc: "True", WorkEnd: self.selectedWS(), WorkStart: self.selectedWE() }, function (data, textStatus) {
                self.prices(ko.utils.arrayMap(data, function (item) {
                    return new PriceListModel(item);
                }));
                resolve();
            });
        })
 
           



        //return promise;
        //var self = this;
        //var deferred = $.Deferred();


      
        //    deferred.resolve();



        //}, "json");
        //return deferred.promise();


    }

    ViewModel.prototype.AddItemToEnd = function () {
        this.items.push(new ItemModel({},this));
    }
    ViewModel.prototype.AddItemToTop = function () {
        this.items.unshift(new ItemModel({}, this));
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
        var data = self.ToJSON();
            self.postAllowed(false);
        $.ajax({
            url: postUrl,
            type: 'POST',
            dataType: 'json',
            data: data,
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
    vm.GetAVRList().then(function () { vm.avrListLoaded(true); });

    //var avrData = $.getJSON("/Json/GetAVRList", function (data) {
    //    ko.applyBindings(new ViewModel(data));
    //})

    //ko.applyBindings(new ViewModel());

})