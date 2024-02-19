<template>
  <div class="social" v-loading.fullscreen.lock="fullscreenLoading">
    <component :is="view" v-if="view"></component>
  </div>
</template>
<style scoped lang="scss">
</style>
<script>
const asyncComponent = require("../components/async-component.tsx").default;
export default {
  data() {
    return {
      fullscreenLoading: true,
      view: undefined
    };
  },
  methods: {
    async login(options) {
      const params = Object.assign({}, options);
      const data = await this.$ajax.get(
        `v1/account/callback?${this.$qs.stringify(params)}`
      );
      this.$store.dispatch("UserLogin", data);
      if (gtag) {
        gtag("event", "QQ登录", {
          event_category: data.data.userName
        });
      }
      this.fullscreenLoading = false;

      if (!data.data.email) {
        let com = require("./../components/set-email").default;
        try {
          await asyncComponent(
            com,
            {
              props: {
                isMobile: this.isMobile
              }
            },
            template => {
              this.view = template;
            }
          );
          this.view = undefined;
        } catch (e) {
          this.view = undefined;
        }
      }

      this.$message.success("登录成功");
      this.$store.dispatch("setDialogName", "setAddress");
      this.$router.replace("/");
    }
  },
  async created() {
    const query = this.$route.query;
    if (query.code && query.state) {
      this.fullscreenLoading = true;
      this.login({
        code: query.code,
        state: query.state
      });
    } else {
      this.fullscreenLoading = false;
      this.$router.replace("/");
    }
  },
  mounted() {}
};
</script>