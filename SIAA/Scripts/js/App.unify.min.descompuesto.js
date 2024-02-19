"use strict";
angular.module("App", ["ui.bootstrap", "ngSanitize", "schemaForm", "App.Controllers", "App.Controllers.Modal"]), angular.module("App"),
    function () {
        angular.module("App.Controllers.Modal", []), angular.module("App.Controllers.Modal").controller("agregarArticuloCtrl", ["$uibModalInstance", "dataIO", "$filter", "$http", function (t, o, e, r) {
            var a = this;
            a.data = o, a.agregar = function () {
                var o = $("#imgProducto").get(0);
                a.data.articuloSelect.fileIMG = o.files[0];
                var e, l;
                r({
                    method: "POST",
                    url: "/Articulo/addArticulo",
                    data: (e = a.data.articuloSelect, l = new FormData, angular.forEach(e, function (t, o) {
                        l.append(o, t)
                    }), l),
                    transformRequest: angular.identity,
                    headers: {
                        "Content-Type": void 0
                    }
                }).then(function (o) {
                    console.log(o), 200 == o.status && (swal("", "El nuevo Articulo se Agrego correctamente!! ", "success"), t.close(a.data))
                }, function (t) {
                    console.log(t), swal("", "Ocurrio un Error al intentar Agregar el Articulo", "error")
                })
            }
        }]).controller("editarArticuloCtrl", ["$uibModalInstance", "dataIO", "$filter", "$http", function (t, o, e, r) {
            var a = this;
            a.data = o, a.actualizar = function () {
                var o = $("#imgProducto").get(0);
                a.data.articuloSelect.fileIMG = o.files[0];
                var e, l;
                r({
                    method: "POST",
                    url: "/Articulo/updateArticulo",
                    data: (e = a.data.articuloSelect, l = new FormData, angular.forEach(e, function (t, o) {
                        l.append(o, t)
                    }), l),
                    transformRequest: angular.identity,
                    headers: {
                        "Content-Type": void 0
                    }
                }).then(function (o) {
                    console.log(o), 200 == o.status && (swal("", "La actualizacion se realizo correctamente!! ", "success"), t.close(a.data.articuloSelect))
                }, function (t) {
                    console.log(t), swal("", "Ocurrio un Error al Actualizar el Articulo", "error")
                })
            }
        }]).controller("matrizPreciosCtrl", ["$uibModalInstance", "$uibModal", "dataIO", "$filter", "$http", function (t, o, e, r, a) {
            var l = this;
            l.data = e, console.log(l.data), l.AddModal = function () {
                o.open({
                    templateUrl: "templ_AddRowMatrizPrecio.html",
                    controller: "AddRowMatrizPrecio",
                    controllerAs: "vm",
                    size: "sm",
                    resolve: {
                        dataIO: function () {
                            return l.data.IDA
                        }
                    }
                }).result.then(function (t) {
                    null != t && l.data.MatrizPrecios.push(t)
                }, function () {
                    console.info("Modal Add Row Matriz Precio Cancelado: " + new Date)
                })
            }, l.AddUpdateRow = function (t) {
                a({
                    method: "POST",
                    url: "/Articulo/AddUpdateRowMatrizPrecio",
                    data: {
                        rowMatrizPrecio: t
                    }
                }).then(function (t) {
                    200 == t.status && swal("", "El Registro fue Actualizado!! ", "success")
                }, function (t) {
                    console.log(t), swal("", "Ocurrio un Error al actualizar el Registro!! ", "error")
                })
            }, l.deleteRow = function (t, o) {
                swal({
                    title: "",
                    text: "Esta segur@ que desea eliminar el Registro? ",
                    icon: "warning",
                    buttons: !0,
                    dangerMode: !0
                }).then(function (e) {
                    e && a({
                        method: "POST",
                        url: "/Articulo/deleteRowRowMatrizPrecio",
                        data: {
                            id: o.idMatrizPrecio
                        }
                    }).then(function (o) {
                        200 == o.status && (l.data.MatrizPrecios.splice(t, 1), swal("", "El Registro fue Eliminado!! ", "success"))
                    }, function (t) {
                        console.log(t), swal("", "Ocurrio un Error al eliminar el Registro!! ", "error")
                    })
                })
            }
        }]).controller("AddRowMatrizPrecio", ["$uibModalInstance", "$uibModal", "dataIO", "$filter", "$http", function (t, o, e, r, a) {
            var l = this;
            l.nRow = {}, l.nRow.IDArticulo = e, l.AddUpdateRow = function () {
                a({
                    method: "POST",
                    url: "/Articulo/AddUpdateRowMatrizPrecio",
                    data: {
                        rowMatrizPrecio: l.nRow
                    }
                }).then(function (o) {
                    200 == o.status && (swal("", "El Row de Matriz de Precio se Agrego Exitosamente", "success"), t.close(l.nRow))
                }, function (t) {
                    console.log(t), swal("", "Ocurrio un Error al eliminar el Registro!! ", "error")
                })
            }
        }]).controller("listPresentacionesCtrl", ["$uibModalInstance", "$uibModal", "dataIO", "$http", function (t, o, e, r) {
            var a = this;
            a.data = e, a.AddEditPresentacion = function (t) {
                var e = {};
                e.Articulo = a.data.Articulo, e.Presentacion = t, e.Schema = a.data.Schema, o.open({
                    templateUrl: "templ_PresentacionProducto.html",
                    controller: "featureFamilyCtrl",
                    controllerAs: "vm",
                    size: "md",
                    resolve: {
                        dataIO: function () {
                            return e
                        }
                    }
                }).result.then(function (t) {
                    r({
                        method: "POST",
                        url: "/Articulo/GetListPresentaciones_Schema",
                        data: {
                            Art: e.Articulo
                        }
                    }).then(function (t) {
                        200 == t.status && (a.data.lPrecentacion = t.data.lPrecentacion)
                    }, function (t) {
                        console.log(t), swal("", "Ocurrio error al obtener la Lista de Presentaciones!! ", "error")
                    })
                }, function () {
                    console.info("Modal Add Row Matriz Precio Cancelado: " + new Date)
                })
            }, a.deletePresentacion = function (t, o) {
                swal({
                    title: "",
                    text: "Esta segur@ que desea eliminar la Presentacion? ",
                    icon: "warning",
                    buttons: !0,
                    dangerMode: !0
                }).then(function (e) {
                    e && r({
                        method: "POST",
                        url: "/Articulo/deletePresentacion",
                        data: {
                            ID: o.ID
                        }
                    }).then(function (o) {
                        200 == o.status && (a.data.lPrecentacion.splice(t, 1), swal("", o.data.StatusDescription, "success"))
                    }, function (t) {
                        console.log(t), swal("", t.data.StatusDescription, "error")
                    })
                })
            }
        }]).controller("matrizCostosCtrl", function (t, o, e, r, a) {
            var l = this;
            l.data = e, console.log(l.data), l.AddModal = function () {
                o.open({
                    templateUrl: "templ_AddRowMatrizCosto.html",
                    controller: "AddRowMatrizCosto",
                    controllerAs: "vm",
                    size: "sm",
                    resolve: {
                        dataIO: function () {
                            return l.data.IDA
                        }
                    }
                }).result.then(function (t) {
                    null != t && l.data.MatrizCostos.push(t)
                }, function () {
                    console.info("Modal Add Row Matriz Costo Cancelado: " + new Date)
                })
            }, l.AddUpdateRow = function (t) {
                a({
                    method: "POST",
                    url: "/Articulo/AddUpdateRowMatrizCosto",
                    data: {
                        rowMatrizCosto: t
                    }
                }).then(function (t) {
                    200 == t.status && swal("", "El Registro fue Actualiado!! ", "success")
                }, function (t) {
                    console.log(t), swal("", "Ocurrio un Error al actualiar el Registro!! ", "error")
                })
            }, l.deleteRow = function (t, o) {
                swal({
                    title: "",
                    text: "Esta segur@ que desea eliminar el Registro? ",
                    icon: "warning",
                    buttons: !0,
                    dangerMode: !0
                }).then(function (e) {
                    e && a({
                        method: "POST",
                        url: "/Articulo/deleteRowRowMatrizCosto",
                        data: {
                            id: o.idMatrizCosto
                        }
                    }).then(function (o) {
                        200 == o.status && (l.data.MatrizCostos.splice(t, 1), swal("", "El Registro fue Eliminado!! ", "success"))
                    }, function (t) {
                        console.log(t), swal("", "Ocurrio un Error al eliminar el Registro!! ", "error")
                    })
                })
            }
        }).controller("AddRowMatrizCosto", function (t, o, e, r, a) {
            var l = this;
            l.nRow = {}, l.nRow.IDArticulo = e, l.AddUpdateRow = function () {
                a({
                    method: "POST",
                    url: "/Articulo/AddUpdateRowMatrizCosto",
                    data: {
                        rowMatrizCosto: l.nRow
                    }
                }).then(function (o) {
                    200 == o.status && (swal("", "El Row de Matriz de Costo se Agrego Exitosamente", "success"), t.close(l.nRow))
                }, function (t) {
                    console.log(t), swal("", "Ocurrio un Error al eliminar el Registro!! ", "error")
                })
            }
        }).controller("featureFamilyCtrl", ["$http", "dataIO", "$uibModalInstance", function ($http, dataIO, $uibModalInstance) {
            var vm = this,
                ID, IDPresentacion;
            vm.data = {}, vm.model = {}, vm.data.Articulo = dataIO.Articulo, null != dataIO.Presentacion && (vm.data.Model = JSON.parse(JSON.stringify(eval("(" + dataIO.Presentacion.jsonPresentacion + ")"))), vm.model = vm.data.Model, ID = dataIO.Presentacion.ID, IDPresentacion = dataIO.Presentacion.IDPresentacion), vm.data.Schema = JSON.parse(dataIO.Schema), vm.texto = null != dataIO.Presentacion ? "Editar" : "Agregar", vm.schema = {
                type: "object",
                properties: vm.data.Schema
            }, vm.myform = ["*"], vm.addUpdateModel = function () {
                var t = {
                    ID: ID,
                    IDPresentacion: IDPresentacion,
                    Articulo_IDArticulo: vm.data.Articulo.IDArticulo,
                    jsonPresentacion: JSON.stringify(vm.model)
                };
                $http({
                    method: "POST",
                    url: "/Articulo/addUpdatePresentacion",
                    data: t
                }).then(function (t) {
                    200 == t.status && (swal("", t.data.StatusDescription, "success"), $uibModalInstance.close())
                }, function (t) {
                    console.log(t), swal("", "Ocurrio un Error al Agregar el Registro!! ", "error")
                })
            }
        }])
    }(), angular.module("App.Controllers", []), angular.module("App.Controllers").controller("ListarArticulosCtrl", ["$scope", "$filter", "$http", "$uibModal", function (t, o, e, r) {
        var a = this;
        a.modelsArticulo = {}, a.modelsArticulo.articuloSelect = [], a.modelsArticulo.cmb = {}, a.modelsArticulo.cmb.listTipoArticulos = [], a.modelsArticulo.cmb.listFamilia = [], a.modelsArticulo.cmb.listMonedas = [], a.modelsArticulo.cmb.listInspenccion = [], a.modelsArticulo.cmb.listMuestreo = [], a.modelsArticulo.cmb.listAQLCalidad = [], a.modelsArticulo.cmb.listClaveUnidad = [], a.filtro = {}, a.filtro.texto = "", a.filtro.cmbTArticulo = "", a.filtro.cmbTFamilia = "", a.listTipoArticulos = [], a.listFamilia = [], a.listMonedas = [], a.listInspenccion = [], a.listMuestreo = [], a.listAQLCalidad = [], a.listClaveUnidad = [], a.listaArticulos = [], a.data = {}, a.viewby = 5, a.totalItems = {}, a.currentPage = 1, a.itemsPerPage = a.viewby, a.maxSize = 10, a.showLoading = !1, a.init = function () {
            a.showLoading = !0, e({
                method: "POST",
                url: "/Articulo/getListArticulos"
            }).then(function (t) {
                a.showLoading = !1, a.listaArticulos = t.data, a.data = a.listaArticulos, a.totalItems = a.data.length
            }, function (t) {
                a.showLoading = !1, console.log(t), swal("", "Ocurrio un Error al obtener el Listado de Articulos ", "error")
            }), a.showLoading = !0, e({
                method: "POST",
                url: "/Articulo/getParametrosArticulos"
            }).then(function (t) {
                a.showLoading = !1, a.listTipoArticulos = t.data.TipoArticulos, a.listFamilia = t.data.Familias, a.listMonedas = t.data.Monedas, a.listInspenccion = t.data.Inspenccion, a.listMuestreo = t.data.Muestreo, a.listAQLCalidad = t.data.AQLCalidad, a.listClaveUnidad = t.data.c_ClaveUnidad, a.modelsArticulo.cmb.listTipoArticulos = a.listTipoArticulos, a.modelsArticulo.cmb.listFamilia = a.listFamilia, a.modelsArticulo.cmb.listMonedas = a.listMonedas, a.modelsArticulo.cmb.listInspenccion = a.listInspenccion, a.modelsArticulo.cmb.listMuestreo = a.listMuestreo, a.modelsArticulo.cmb.listAQLCalidad = a.listAQLCalidad, a.modelsArticulo.cmb.listClaveUnidad = a.listClaveUnidad
            }, function (t) {
                a.showLoading = !1, console.log(t), swal("", "Ocurrio un Error al obtener los Parametros de Articulo", "error")
            })
        }, a.filtrar = function () {
            console.log(a.filtro.cmbTArticulo), e({
                method: "POST",
                url: "/Articulo/getListArticulosFiltro",
                data: {
                    filtro: a.filtro
                }
            }).then(function (t) {
                a.listaArticulos = t.data, a.data = a.listaArticulos, a.totalItems = a.data.length
            }, function (t) {
                console.log(t), swal("", "Ocurrio un Error al Generar el Filtro de Articulos", "error")
            })
        }, t.$watch("lA.filtro.texto", function () {
            a.data = o("filter")(a.listaArticulos, a.filtro.texto), a.totalItems = a.data.length
        }), a.setPage = function (t) {
            a.currentPage = t
        }, a.pageChanged = function () {
            console.log("Page changed to: " + a.currentPage)
        }, a.setItemsPerPage = function (t) {
            a.itemsPerPage = t, a.currentPage = 1
        }, a.infoAcciones = function () {
            alertify.alert("<b> Como Funciona? </b>", "Elija el Regristro que desea, Haga Click en el DropDownList y seleccione una Accion")
        }, a.crearArticulo = function () {
            a.modelsArticulo.articuloSelect = {}, r.open({
                templateUrl: "templ_addProducto.html",
                controller: "agregarArticuloCtrl",
                controllerAs: "vm",
                size: "lg",
                resolve: {
                    dataIO: function () {
                        return a.modelsArticulo
                    }
                }
            }).result.then(function (t) {
                a.modelsArticulo.articuloSelect = t
            }, function () {
                console.info("Modal Editar Producto Cancelado: " + new Date)
            })
        }, a.accionesList = function (t, o, l) {
            switch (o[t]) {
                case "E":
                    e({
                        method: "POST",
                        url: "/Articulo/GetModelArticuloXID",
                        data: {
                            IDA: l.IDArticulo
                        }
                    }).then(function (o) {
                        a.modelsArticulo.articuloSelect = o.data, r.open({
                            templateUrl: "templ_editarProducto.html",
                            controller: "editarArticuloCtrl",
                            controllerAs: "vm",
                            size: "lg",
                            resolve: {
                                dataIO: function () {
                                    return a.modelsArticulo
                                }
                            }
                        }).result.then(function (o) {
                            void 0 != o && (a.listaArticulos[t].IDArticulo = o.IDArticulo, a.listaArticulos[t].Descripcion = o.Descripcion, e({
                                method: "POST",
                                url: "/Articulo/getArticulosxIDP",
                                data: {
                                    IDA: o.IDArticulo
                                }
                            }).then(function (o) {
                                a.listaArticulos[t] = o.data[0]
                            }, function (t) {
                                console.log(t), swal("", "Ocurrio un Error al Obtener los Datos!! ", "error")
                            }))
                        }, function () {
                            console.info("Modal Editar Producto Cancelado: " + new Date)
                        })
                    }, function (t) {
                        console.log(t), swal("", "Ocurrio un Error al Obtener los Datos!! ", "error")
                    });
                    break;
                case "P":
                    e({
                        method: "POST",
                        url: "/Articulo/GetListPresentaciones_Schema",
                        data: {
                            Art: l
                        }
                    }).then(function (t) {
                        var o = t.data;
                        o.Articulo = l, r.open({
                            templateUrl: "templ_listPresentaciones.html",
                            controller: "listPresentacionesCtrl",
                            controllerAs: "vm",
                            size: "lg",
                            resolve: {
                                dataIO: function () {
                                    return o
                                }
                            }
                        })
                    }, function (t) {
                        console.log(t), swal("", "Ocurrio un Error al Obtener los Datos!! ", "error")
                    });
                    break;
                case "MP":
                    e({
                        method: "POST",
                        url: "/Articulo/GetMatrizPrecios",
                        data: {
                            id: l.IDArticulo
                        }
                    }).then(function (t) {
                        var o = {};
                        o.IDA = l.IDArticulo, o.MatrizPrecios = t.data, r.open({
                            templateUrl: "templ_listaMatrizPrecio.html",
                            controller: "matrizPreciosCtrl",
                            controllerAs: "vm",
                            size: "lg",
                            resolve: {
                                dataIO: function () {
                                    return o
                                }
                            }
                        })
                    }, function (t) {
                        console.log(t), swal("", "Ocurrio un Error al Obtener los Datos!! ", "error")
                    });
                    break;
                case "MC":
                    e({
                        method: "POST",
                        url: "/Articulo/GetMatrizCostos",
                        data: {
                            id: l.IDArticulo
                        }
                    }).then(function (t) {
                        var o = {};
                        o.IDA = l.IDArticulo, o.MatrizCostos = t.data, r.open({
                            templateUrl: "templ_listaMatrizCosto.html",
                            controller: "matrizCostosCtrl",
                            controllerAs: "vm",
                            size: "lg",
                            resolve: {
                                dataIO: function () {
                                    return o
                                }
                            }
                        })
                    }, function (t) {
                        console.log(t), swal("", "Ocurrio un Error al Obtener los Datos!! ", "error")
                    });
                    break;
                default:
                    swal("Ups", "Opcion no Valida!!, ni existe !! ", "error")
            }
            o[t] = ""
        }
    }]);