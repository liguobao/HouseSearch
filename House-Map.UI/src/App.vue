<template>
  <div id="app">
    <router-view/>
  </div>
</template>
<style lang="scss">
  *{
    padding: 0;
    margin: 0;
    -webkit-box-sizing: border-box;
    -moz-box-sizing: border-box;
    box-sizing: border-box;
  }
  ul,li{
    list-style: none;
  }
  a{
    text-decoration: none;
  }
  body{
    font-weight: 400;
    line-height: 1.6;
    font-family: "Segoe UI","Lucida Grande",Helvetica,Arial,"Microsoft YaHei",FreeSans,Arimo,"Droid Sans","wenquanyi micro hei","Hiragino Sans GB","Hiragino Sans GB W3",FontAwesome,sans-serif;
  }
  @font-face {
    font-family: 'iconfont';  /* project id 826486 */
    src: url('//at.alicdn.com/t/font_826486_ns6jc4zbuh.eot');
    src: url('//at.alicdn.com/t/font_826486_ns6jc4zbuh.eot?#iefix') format('embedded-opentype'),
    url('//at.alicdn.com/t/font_826486_ns6jc4zbuh.woff') format('woff'),
    url('//at.alicdn.com/t/font_826486_ns6jc4zbuh.ttf') format('truetype'),
    url('//at.alicdn.com/t/font_826486_ns6jc4zbuh.svg#iconfont') format('svg');
  }
  .ellipsis{
    display: -webkit-box;
    text-overflow: ellipsis;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }
  .text-center{
    text-align: center;
  }
  .text-left{
    text-align: left;
  }
  .text-right{
    text-align: right;
  }
  ::-webkit-scrollbar-track-piece {
    background-color: #f8f8f8;
  }

  ::-webkit-scrollbar {
    width: 9px;
    height: 9px;
  }

  ::-webkit-scrollbar-thumb {
    background-color: #dddddd;
    background-clip: padding-box;
    min-height: 28px;
  }

  ::-webkit-scrollbar-thumb:hover {
    background-color: #bbb;
  }
  html,body{
    width: 100%;
  }
  /*body{*/
    /*overflow-x: hidden;*/
  /*}*/
  .el-dialog__wrapper{
    -webkit-overflow-scrolling: touch;
  }
</style>
<script>
  export default {
    created() {
      const cnzz_protocol = (("https:" == document.location.protocol) ? " https://" : " http://");
      document.write(unescape("%3Cspan style='display:none' id='cnzz_stat_icon_1260881876'%3E%3C/span%3E%3Cscript src='" +
        cnzz_protocol + "s95.cnzz.com/z_stat.php%3Fid%3D1260881876' type='text/javascript'%3E%3C/script%3E"));
    }
  }
</script>


