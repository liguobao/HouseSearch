
//输入框自动补充
$(function(){
	$("#SearchKey").focus();
	$("input[name='searchkw']").unautocomplete();
    $("input[name='searchkw']").autocomplete(sitepath+'?c=api&a=search',{
		minChars: 1,
		width: 346,
		matchContains: true,
		autoFill: false,
		resultsClass :'search_input' ,
		matchCase:false,
		selectFirst: false,
		formatItem:function(row,i,max){
			return row[0];
		}
	}).result(function(event, data, formatted){
		formatted=formatted.replace(/<[\s\S]*?>/ig, "");
		$("input[name='searchkw']").val(formatted);
	});
});

//提交搜索表单
function search_post() {
   var kw=$('#SearchKey').val();
   if (kw) {
	   var url=sitepath+'?c=content&a=search&kw='+kw;
	   window.location.href=url;
	   return false;
   } else {
      return false;
   }
}

//会员收藏夹
function addfavorite(id, obj) {
    $('#'+obj).html('处理中');
	$.post(sitepath+'?c=api&a=addfavorite&r='+Math.random(), { id:id }, function(data){
		$('#'+obj).html(data); 
	});
}
