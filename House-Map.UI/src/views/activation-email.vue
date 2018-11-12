<template>
  <div>

  </div>
</template>
<script>
  export default {
    methods:{
      async activation(code) {
        const data = await this.$ajax.get(`/account/activated/${code}`);
        if (gtag) {
          gtag('event', '邮箱激活', {
            'event_category': data.data.userName
          });
        }
        this.$store.dispatch('UserLogin', data);
        this.$message.success(data.message ? data.message : '登录成功');
        this.$router.replace('/');
      }
    },
    created() {
      let code = this.$route.query.code;
      if(code) {
        this.activation(code)
      }
    }
  }
</script>
<style>

</style>