<template>
  <div class="login-wrap">
    <login class="content" @close="close"></login>
  </div>
</template>
<script>
  import login from './../components/login'
  export default {
    components:{
      login
    },
    methods:{
      close() {
        this.$router.replace('/')
      }
    }
  }
</script>
<style scoped lang="scss">
  .login-wrap{
    background: #ececec;
    height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
    .content{
      width: 500px;
      height: 600px;
      box-shadow: 0 2px 12px 0 rgba(0,0,0,.1);
      border: 1px solid #ebeef5;
      background: #fff;
      display: flex;
      align-items: center;
      flex-direction: column;
      justify-content: center;
      padding: 40px;
    }
  }
</style>