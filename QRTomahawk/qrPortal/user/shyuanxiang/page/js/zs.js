/**
Author:王玉坤
**/
window.onload = window.onresize = window.onscroll = function ()
{
 var oBox = document.getElementById("qqbox_zzjs");
 var iScrollTop = document.documentElement.scrollTop || document.body.scrollTop;
 setTimeout(function ()
 {
  clearInterval(oBox.timer);
  var iTop = parseInt((document.documentElement.clientHeight - oBox.offsetHeight)/2) + iScrollTop;
  oBox.timer = setInterval(function ()
  {
   var iSpeed = (iTop - oBox.offsetTop) / 8;
   iSpeed = iSpeed > 0 ? Math.ceil(iSpeed) : Math.floor(iSpeed);
   oBox.offsetTop == iTop ? clearInterval(oBox.timer) : (oBox.style.top = oBox.offsetTop + iSpeed + "px");
  }, 30);
 }, 100);
 
};

$(document).ready(function(){	
	
	 $('#zs_menu').click(function () {
		$("body").append("<div id='mask'></div>");
		$("#mask").addClass("mask").fadeIn("slow");
		$("#LoginBox").fadeIn("slow");
		  var re = getLT(200,200);  
		 // alert(re[0]);
		  document.getElementById("LoginBox").style.left = re[0] + 'px';  
		  document.getElementById("LoginBox").style.top = re[1] + 'px';  
		  
	});	
		
});


function getLT(_w,_h)  
{  
   var de = document.documentElement;  
  
   // 获取当前浏览器窗口的宽度和高度  
   // 兼容写法，可兼容ie,ff  
   var w = self.innerWidth || (de&&de.clientWidth) || document.body.clientWidth;  
   var h = (de&&de.clientHeight) || document.body.clientHeight;  
  
   // 获取当前滚动条的位置  
   // 兼容写法，可兼容ie,ff  
   var st= (de&&de.scrollTop) || document.body.scrollTop;  
  
   var topp=0;  
   if(h>_h)  
     topp=(st+(h - _h)/2);  
   else  
     topp=st;  
  
   var leftp = 0;  
   if(w>_w)  
     leftp = ((w - _w)/2);  
  
   // 左侧距，顶部距  
   return [leftp,topp];  
}  