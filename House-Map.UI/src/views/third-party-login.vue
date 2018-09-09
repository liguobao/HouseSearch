<template>
  <div>
    第三方登录中...
  </div>
</template>
<script>
  export default {
    methods: {
      async login(options) {
        const params = Object.assign({}, options);
        const data = await this.$ajax.get(`/account/callback?${this.$qs.stringify(params)}`);
        this.$store.dispatch('UserLogin', data);
        this.$router.replace('/')
      }
    },
    created() {
      const query = this.$route.query;
      if(query.code && query.state) {
        this.login({
          code: query.code,
          state: query.state
        })
      }
    }
  }
</script>