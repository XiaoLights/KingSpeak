var applicationIndex = new function () {
    var Current = this;
    this.Init = function () {
        Current.InitTable();
        Current.BtnClick();
    }

    this.InitTable = function () {
        $('#table').bootstrapTable({
            url: '/Admin/Application/GetAppTokenList',                           //请求后台的URL（*）
            method: 'post',                     //请求方式（*）
            toolbar: '#toolbar',                   //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            pagination: true,                   //是否显示分页（*）
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            queryParams: Current.GetParams,//传递参数（*）
            pageSize: 10,                       //每页的记录行数（*）
            pageList: [10, 20, 30],            //可供选择的每页的行数（*）
            columns: [{
                checkbox: true,
                align: 'center'
            }, {
                field: 'ID',
                title: '应用编号'
                , sortable: true
            }, {
                field: 'AppName',
                title: '应用名称'

            }, {
                field: 'AppDescripts',
                title: '应用描述'

            }, {
                field: 'AppToken',
                title: '接入秘钥'
            }, {
                field: 'CreateDate',
                title: '创建时间'
                , sortable: true
                , formatter: function (value) { return Common.FormatTime(value); }
            }, {
                field: 'State',
                title: '状态'
                 , sortable: true
                 , formatter: function (value) {
                     if (value == '1') {
                         return '正常';
                     }
                     else {
                         return '禁用';
                     }
                 }
            }, {
                field: 'ExpirDate',
                title: '有效期'
                 , sortable: true
            }, {
                field: 'UserId',
                title: '操作'
                 , formatter: Current.GetFunHtml
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

    this.GetFunHtml = function (value, row) {
        var html = '';
        if (row.State == "1") {
            var t = "'" + row.ID + "','2'";
            html += '<a href="javacript:void(0)" onclick="applicationIndex.ChangeState(' + t + ')">禁用</a>';
        }
        else {
            var t = "'" + row.ID + "','1'";
            html += '<a href="javacript:void(0)" onclick="applicationIndex.ChangeState(' + t + ')">启用</a>';
        }
        return html;
    }

    this.ChangeState = function (id, state) {
        var obj = { ID: id, State: state };
        $.post("/Admin/Application/ChangeState", obj, function (data) {
            if (data.Success) {
                layer.msg((state == "1" ? "启用" : "禁用") + "成功");
                $('#table').bootstrapTable("refresh");
            }
            else {
                alert(data.ErrorMsg);
            }
        })
    }

    this.BtnClick = function () {
        $("#btn_search").click(function () {
            $('#table').bootstrapTable('selectPage', 1);
        })
        $("#btn_add").click(function () {
            layer.open({
                content: '测试回调',
                yes: function (index, layero) {
                    //do something
                    layer.close(index); //如果设定了yes回调，需进行手工关闭
                }
            });
             
        })
    }
}
$(function () {
    applicationIndex.Init();
})