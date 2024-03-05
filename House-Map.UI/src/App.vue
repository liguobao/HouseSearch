<template>
  <div id="app">
    <a v-if="isMobile && $route.name !== 'home'" href="https://wj.qq.com/s/2953926/aabe" target="_blank" class="do-more-better">帮我们做得更好?</a>
    <transition name="fade" >
      <router-view @wx="toWX" />
    </transition>
    <el-dialog
            top="50px"
            width="90%"
            title="系统提示"
            :visible.sync="toWx"
            append-to-body
            center
    >
      <div class="to-wx">
        <img src="https://house2048.cn/app/house-map/resource/img/ewm.aece70b1.jpg">
        <p>
          为了更好的找房体验，
        </p>
        <p>
          移动端功能全部迁移到小程序，
        </p>
        <P>
          “地图搜租房”，
        </P>
        <p>
          关注【人生删除指南】微信公众号，
        </p>
        <P>
          获取地址一键进入，
        </P>
        <p>
          更多功能更新一秒便知。
        </p>
      </div>
    </el-dialog>
  </div>
</template>

<style scoped lang="scss">
  .do-more-better{
    text-align: center;
    background: rgba(0,0,0,0.5);
    display: block;
    position: fixed;
    z-index: 2500;
    left: 0;
    top: 0;
    width: 100%;
    padding: 4px 0;
    color: #fff;
    font-size: 12px;
    text-decoration: underline;
  }
</style>
<style lang="scss">
  * {
    padding: 0;
    margin: 0;
    -webkit-box-sizing: border-box;
    -moz-box-sizing: border-box;
    box-sizing: border-box;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }

  ul, li {
    list-style: none;
  }

  a {
    text-decoration: none;
  }

  body {
    font-weight: 400;
    line-height: 1.6;
  }

  @font-face {
    font-family: 'FontAwesome';
    src: url('./fonts/fontawesome-webfont.eot?v=4.6.3');
    src: url('./fonts/fontawesome-webfont.eot?#iefix&v=4.6.3') format('embedded-opentype'), url('./fonts/fontawesome-webfont.woff2?v=4.6.3') format('woff2'), url('./fonts/fontawesome-webfont.woff?v=4.6.3') format('woff'), url('./fonts/fontawesome-webfont.ttf?v=4.6.3') format('truetype');
    font-weight: normal;
    font-style: normal;
  }

  @font-face {
    font-family: 'iconfont';  /* project id 826486 */
    src: url('//at.alicdn.com/t/font_826486_ns6jc4zbuh.eot');
    src: url('//at.alicdn.com/t/font_826486_ns6jc4zbuh.eot?#iefix') format('embedded-opentype'),
    url('//at.alicdn.com/t/font_826486_ns6jc4zbuh.woff') format('woff'),
    url('//at.alicdn.com/t/font_826486_ns6jc4zbuh.ttf') format('truetype'),
    url('//at.alicdn.com/t/font_826486_ns6jc4zbuh.svg#iconfont') format('svg');
  }

  .ellipsis {
    display: -webkit-box;
    text-overflow: ellipsis;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }

  .text-center {
    text-align: center;
  }

  .text-left {
    text-align: left;
  }

  .text-right {
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

  html, body {
    width: 100%;
  }

  /*body{*/
  /*overflow-x: hidden;*/
  /*}*/
  .el-dialog__wrapper {
    -webkit-overflow-scrolling: touch;
  }

  .to-wx{
    text-align: center;
    font-size: 0;
    img{
      display: inline-block;
      max-width: 60%;
    }
    p{
      font-size: 16px;
    }
  }
</style>
<script>
  export default {
    watch: {
      '$route'() {
        this.czc();
      },
    },
    computed: {
      isMobile() {
        return this.$store.state.isMobile
      }
    },
    methods: {
      toWX(){
       this.toWx = true;
      },
      czc() {
        if (window._czc) {
          let location = window.location
          let contentUrl = location.pathname + location.hash
          let refererUrl = '/'
          window._czc.push(['_trackPageview', contentUrl, refererUrl])

        } else {
          setTimeout(() => {
            this.czc();
          }, 200)
        }
      },
      isIE(){
        var userAgent = navigator.userAgent; //取得浏览器的userAgent字符串
        var isIE = userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1; //判断是否IE<11浏览器
        var isEdge = userAgent.indexOf("Edge") > -1 && !isIE; //判断是否IE的Edge浏览器
        var isIE11 = userAgent.indexOf('Trident') > -1 && userAgent.indexOf("rv:11.0") > -1;
        if(isIE || isIE11) {
          alert('我们已经不支持IE11内核以下的浏览器，请使用谷歌/火狐浏览器访问。谢谢支持(≧ڡ≦*)')
        }
      }
    },
    data(){
      return{
        toWx:false
      }
    },
    created(){
      this.isIE();
      if(this.isMobile){
        this.toWX();
      }
    },
    mounted() {
      let self = this;
      const script = document.createElement('script')
      script.src = 'https://s95.cnzz.com/z_stat.php?id=1260881876&web_id=1260881876'
      script.language = 'JavaScript'
      document.body.appendChild(script);
      script.onload = function () {
        self.czc();
      };

    }
  }
</script>
