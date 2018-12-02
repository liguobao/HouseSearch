<template>
  <div class="social" v-loading.fullscreen.lock="fullscreenLoading">

  </div>
</template>
<style scoped lang="scss">

</style>
<script>
  export default {
    data() {
      return {
        fullscreenLoading: true
      }
    },
    methods: {
      async login(options) {
        const params = Object.assign({}, options);
        const data = await this.$ajax.get(`/account/callback?${this.$qs.stringify(params)}`);
        this.$store.dispatch('UserLogin', data);
        if (gtag) {
          gtag('event', 'QQ登录', {
            'event_category' : data.data.userName
          });
        }
        this.$message.success('登录成功');
        this.fullscreenLoading = false;
        this.$store.dispatch('setDialogName','setAddress');
        this.$router.replace('/');
      }
    },
    created() {
      const query = this.$route.query;
      if (query.code && query.state) {
        this.fullscreenLoading = true;
        this.login({
          code: query.code,
          state: query.state
        })
      }else {
        this.fullscreenLoading = false;
        this.$router.replace('/')
      }
    },
    mounted() {

    }
  }
</script>