﻿var userAdministrator = new function () {
    var Current = this;
    this.ValidForm;
    this.Init = function () {
        Current.InitTable();
        Current.BtnClick();
        Current.InitFormValidator();
    }

    this.InitTable = function () {
        $('#table').bootstrapTable({
            url: Common.GetRightUrl('/Admin/User/GetAdminList'),                           //请求后台的URL（*）
            method: 'post',                     //请求方式（*）
            toolbar: '#toolbar',                   //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            pagination: true,                   //是否显示分页（*）
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            clickToSelect: true,
            queryParams: Current.GetParams,//传递参数（*）
            pageSize: 10,                       //每页的记录行数（*）
            pageList: [10, 20, 30],            //可供选择的每页的行数（*）
            columns: [{
                checkbox: true,
                align: 'center'
            }, {
                field: 'UserID',
                title: '用户编号'
            }, {
                field: 'UserName',
                title: '用户名称'
                 , sortable: true
            }, {
                field: 'TrueName',
                title: '真实姓名'
            }, {
                field: 'AcatarImg',
                title: '头像'
            }, {
                field: 'CreateDate',
                title: '创建时间'
                 , sortable: true
                 , formatter: function (value) { return Common.FormatTime(value); }
            }, {
                field: 'LastOnlineDate',
                title: '最后登录时间'
                , sortable: true
                 , formatter: function (value) { return Common.FormatTime(value); }
            }, {

                title: '操作',
                events: operateEvents
                , formatter: function (value, row) {
                    return ['<button type="button" class="Editinfo glyphicon glyphicon-edit" title="修改" style="margin-right:15px;"></button>'
                    ].join('');
                }
            }

            ]
        });
    }

    this.GetParams = function (params) {
        var temp = {   //这里的键的名字和控制器的变量名必须一致，这边改动，控制器也需要改成一样的
            pagesize: params.limit,   //页面大小
            pageindex: params.offset//页码
            , SearchType: $("#searchType").val()
            , SearchKey: $("#searchkey").val()
            , sortOrder: params.order//排序
            , sortName: params.sort//排序字段
        };
        return temp;
    }

    this.BtnClick = function () {


        $("#btn_search").click(function () {
            $('#table').bootstrapTable('selectPage', 1);
        })

        $("#btn_add").click(function () {
            Current.ClearDialog();
            Current.OpenDialog();
        })

        $("#btn_del").click(function () {
            Current.DeleteAdminUser();
        })
    }

    this.OpenDialog = function () {
        layer.open({
            title: '添加管理员用户',
            type: 1,
            area: '600px',//大小设置
            fixed: true, //不固定
            btn: ['保存', '放弃'],
            content: $('#addAdmin'),
            btn1: function () {
                //按钮1的回掉（保存按钮)
                Current.SaveAdminUser();
            }
        });

    }

    this.SaveAdminUser = function () {
        var bootstrapValidator = $("#addform").data('bootstrapValidator');
        bootstrapValidator.validate();
        if (!bootstrapValidator.isValid()) {
            return;
        }
        $.post(Common.GetRightUrl("/Admin/User/SaveAdminUser"), $("#addform").serialize(), function (data) {
            if (data.Success) {
                layer.alert("保存成功", { time: 1000 });
                layer.closeAll('page');
                $('#table').bootstrapTable("refresh");
            } else {
                layer.alert(data.ErrorMsg);

            }
        })
    }

    this.DeleteAdminUser = function () {
        layer.confirm('确认要删除选中用户吗？', {
            title: '确认',
            btn: ['确定', '取消'] //按钮
        }, function () {
            var arr = $('#table').bootstrapTable('getSelections');
            if (arr.length < 1) {
                layer.msg("请选择一条记录", { time: 1000 });
                return;
            }
            var idarr = '';
            for (var i = 0; i < arr.length; i++) {
                if (idarr.length != 0) {
                    idarr += ',';
                }
                idarr += arr[i].UserID;
            }
            var obj = { UserIDs: idarr };
            $.post(Common.GetRightUrl('/Admin/User/DeleteAdminUser'), obj, function (data) {
                if (data.Success) {
                    layer.alert("删除成功", { time: 1000 });
                    $('#table').bootstrapTable("refresh");
                }
                else {
                    layer.msg(data.ErrorMsg);
                }
            })

        }, function () {

        });
    }

    this.InitFormValidator = function () {
        $("#addform").bootstrapValidator({
            excluded: [':disabled'],
            feedbackIcons: {
                //valid: 'glyphicon glyphicon-ok',//显示验证成功或者失败时的一个小图标
                //invalid: 'glyphicon glyphicon-remove',
                //validating: 'glyphicon glyphicon-refresh'
            },
            fields: {
                UserName: {
                    message: '请输入用户名！',
                    validators: {
                        notEmpty: {
                            message: '请输入用户名！'
                        },
                        stringLength: {
                            min: 3,
                            max: 20,
                            message: '用户名长度为3-20个字符'
                        }
                    }
                },
                TrueName: {
                    message: '请输入真实姓名！',
                    validators: {
                        notEmpty: {
                            message: '请输入真实姓名！'
                        }
                    }
                },
                PassWord: {
                    message: '请输入密码！',
                    validators: {
                        notEmpty: {
                            message: '请输入密码！'
                        },
                        stringLength: {
                            min: 6,
                            max: 18,
                            message: '密码长度为6-18个字符'
                        }
                    }
                },
                VerifyPwd: {
                    message: '请确认密码！',
                    validators: {
                        notEmpty: {
                            message: '请确认密码！'
                        },
                        identical: {
                            field: 'PassWord',
                            message: '请输入相同的密码'
                        }
                    }
                }
            }
        })
    }

    this.ClearDialog = function () {
        $('#addform').bootstrapValidator('resetForm', true);
    }
}
window.operateEvents = {
    'click .Editinfo': function (e, value, row, index) {
        userAdministrator.ClearDialog();
        $("#hidID").val(row.UserID);
        $("#txtuserName").val(row.UserName);
        $("#txttrueName").val(row.TrueName);
        userAdministrator.OpenDialog();
    }
}


$(function () {
    userAdministrator.Init();
})