<template>
  <el-dialog
    :title="title"
    :width="isMobile ? '100%' : '900px'"
    center
    :visible="visible"
    :append-to-body="appendToBody"
    :before-close="cancel"
  >
    <span
      class="title"
      slot="title"
      @click="navTo({city:title,intervalDay:14})"
      :title="title"
    >{{title}}</span>
    <div>
      <ul class="houses">
        <li v-for="item in houses" :key="item.id">
          <span
            @click="navTo({city:item.city,source:item.source})"
            :title="item.displaySource"
          ><a  @click="navTo({city:item.city,source:item.source})" href="#">{{item.displaySource}}</a></span>
        </li>
      </ul>
    </div>
  </el-dialog>
</template>
<style lang="scss" scoped>
@keyframes toUp {
  0% {
    opacity: 0;
    transform: translateY(50%);
  }
  100% {
    opacity: 1;
    transform: translateY(0);
  }
}
.title {
  color: #409eff;
  text-decoration: underline;
}
.houses {
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
  li {
    margin-right: 30px;
    margin-bottom: 15px;
    a {
      color: #409eff;
      font-size: 14px;
    }
  }
}

@for $i from 1 to 20 {
  li:not(.is-mobile):nth-of-type(#{$i}) {
    animation: toUp 0.5s (0.05s * $i) ease-out both;
  }
}
</style>
<script>
export default {
  props: {
    appendToBody: {
      default: false
    },
    isMobile: {
      default: false
    },
    title: {
      default: ""
    },
    navTo: {}
  },
  data() {
    return {
      visible: true,
      houses: []
    };
  },
  methods: {
    submit() {
      this.close();
      this.$emit("close");
    },
    close() {
      this.visible = false;
    },
    cancel() {
      this.close();
      this.$emit("cancel", false);
    },
    async getList() {
      if (!this.title) {
        return;
      }
      const data = await this.$ajax.get(`/v2/cities/${this.title}`);
      this.houses = data.data;
    }
  },
  created() {
    this.getList();
  }
};
</script>
