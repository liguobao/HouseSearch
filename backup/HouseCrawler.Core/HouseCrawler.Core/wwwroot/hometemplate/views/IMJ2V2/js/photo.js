/*图片频道页面展示JS*/
function getOffset(e)
{
  var target = e.target;
  if (target.offsetLeft == undefined)
  {
    target = target.parentNode;
  }
  var pageCoord = getPageCoord(target);
  var eventCoord =
  {  
    x: window.pageXOffset + e.clientX,
    y: window.pageYOffset + e.clientY
  };
  var offset =
  {
    offsetX: eventCoord.x - pageCoord.x,
    offsetY: eventCoord.y - pageCoord.y
  };
  return offset;
}
function getPageCoord(element)    //计算从触发到root间所有元素的offsetLeft值之和。
{
  var coord = {x: 0, y: 0};
  while (element)
  {
    coord.x += element.offsetLeft;
    coord.y += element.offsetTop;
    element = element.offsetParent;
  }
  return coord;
}
function upNext(bigimg){
	var imgurl  = righturl;
    bigimg.onmousemove=function(e){
    var e=window.event || e,
        posX=(e.offsetX==undefined) ? getOffset(e).offsetX : e.offsetX ;
  if(posX<bigimg.width/2){
            bigimg.style.cursor    = 'url('+siteurl+'images/arr_left.cur),auto';
            imgurl                = lefturl;
        }else{
            bigimg.style.cursor    = 'url('+siteurl+'images/arr_right.cur),auto';
            imgurl                = righturl;
        }
    }
    bigimg.onmouseup=function(){
        location.href=imgurl;
    }
}
function upNext1(bigimg){
	var imgurl		= 1;
	bigimg.onmousemove=function(e){
    var e=window.event || e,
        posX=(e.offsetX==undefined) ? getOffset(e).offsetX : e.offsetX ;
  if(posX<bigimg.width/2){
			bigimg.style.cursor	= 'url('+siteurl+'images/arr_left.cur),auto';
			imgurl				= currPage-1;
		}else{
			bigimg.style.cursor	= 'url('+siteurl+'images/arr_right.cur),auto';
			imgurl				= currPage+1;
		}
	}
	bigimg.onmouseup=function(){
		showImg(imgurl);
	}
}

function addCookie(objName,objValue,objHours){//添加cookie
   var str = objName + "=" + escape(objValue);
   if(objHours > 0){//为0时不设定过期时间，浏览器关闭时cookie自动消失
    var date = new Date();
    var ms = objHours*3600*1000;
    date.setTime(date.getTime() + ms);
    str += "; expires=" + date.toGMTString();
   }
   document.cookie = str;
  }
  
  function getCookie(objName){//获取指定名称的cookie的值
   var arrStr = document.cookie.split("; ");
   for(var i = 0;i < arrStr.length;i ++){
    var temp = arrStr[i].split("=");
    if(temp[0] == objName) return unescape(temp[1]);
   } 
  }

var interval = 5000;
var timerId = -1;
var derId = -1;
var replay = false;
var num = 0;
function forward() {
window.location.href = righturl;
}
function $$(o){
return document.getElementById(o);
}
function derivativeNum() {

}
function playNextPic(stat) {
if(stat || replay) {
derId = window.setInterval('derivativeNum();', 1000);

$$('playid').onclick = function (){replay = false;playNextPic(false);};
$$('playid').innerHTML = '停止播放';
timerId = window.setInterval('forward();', interval);
addCookie("photoautoplayer", true,0);
} else {
addCookie("photoautoplayer", false,0);
replay = true;
num = 0;
$$('playid').innerHTML = '幻灯播放';
$$('playid').onclick = function (){playNextPic(true);};
$$('displayNum').innerHTML = '';
window.clearInterval(timerId);
window.clearInterval(derId);
}
}
window.onload=function(){
 if (getCookie("photoautoplayer")=='true') playNextPic(true);
}

var initnum=4;//每次显示张数,根据页面滚动宽度可适当调整
var scrollWrapW=130;//每次滚动距离

var l;
$(document).ready(function () {
	l=$('#scrool_wrap li').length;
	iss=iss-1;
	
	$("#left").bind("click",ole);
	$("#right").bind("click",ori);
	var total = $('#scrool_wrap li').length;
	$("#zys").html(total); 
     
	
	$('#scrool_wrap li').eq(iss).find('img').eq(0).addClass('curimg');
	
	if(iss>0&&iss<initnum){
		var tiss=iss;
		var temscr=-scrollWrapW*(tiss);
		$("#scrool_wrap").animate({left:temscr},1000);
	}else{
		if(iss<=l-initnum){
			var tiss=iss;
			var temscr=-scrollWrapW*(tiss);
			$("#scrool_wrap").animate({left:temscr},1000);
		}else{
			var tiss=l-initnum;
			var temscr=-scrollWrapW*(tiss);
			$("#scrool_wrap").animate({left:temscr},1000);
			iss=tiss;
		}
	}
	
});

function ole(){
	if(iss==-1){
		iss=0;
	}else if(iss==(l-initnum)){
		olend();
	}else if(iss<(l-initnum)){
		var temscr=-scrollWrapW*(iss+1);
		$("#scrool_wrap").animate({left:temscr},1000);
		iss++;
	}
	
}
 function ori(){
 	if(iss>0){
		var temscr=-scrollWrapW*(iss-1);
		$("#scrool_wrap").animate({left:temscr},1000);
		iss--;
	}else {
		olend();
	}
 }
function  olend(){alert("您已浏览完全部缩略图");}
function  orend(){alert("您已浏览完全部缩略图");}
function prev(){showImg(currPage-1);}
function next(){showImg(currPage+1);}
function showImg(n){
	if (n<=0) return;
	if (n>totalput) n=1;
	 currPage=n;
	  $("#scrool_wrap").find("a").attr("class","normalthumb");
	  $("#t"+n).addClass("currthumb");
	  $("#currpa").html(n);
	  $("#currp").html(n);
	  $("#ShowLargeImg").hide().attr("src",imgArr[n-1]).fadeIn('slow');
	  $(".imageintro").html(introArr[n-1]);
}
//改变图片大小
function resizepic(thispic) { 
    if(thispic.width>750) thispic.width=750; 
}