<template>
  <el-dialog
      title="高级搜索功能"
      :visible.sync="visible"
      :width="isMobile ? '100%' : '550px'"
      center
      :before-close="close"
  >
    <el-dialog
        top="50px"
        :width="isMobile ? '100%' : '70%'"
        title="房源"
        :visible.sync="searchRes"
        append-to-body
        center
        :before-close="closeSearchList"
    >
      <house-search-list type="all" :house-list="houseList" :options="form"  :get-houses-list="getHousesList" key="all" ref="search-list"></house-search-list>
    </el-dialog>
    <el-form ref="form" :model="form" :label-width="isMobile ? '0px' : '130px'" class="form" :rules="rules">
      <el-form-item  :label="isMobile ? '' : '地区'" prop="cityName">
        <el-select v-model="form.cityName" placeholder="请选择地区" style="width: 100%" filterable allow-create>
          <!--<el-option label="全部" value=""></el-option>-->
          <el-option
              v-for="item in cities"
              :key="item.id"
              :label="item.name"
              :value="item.name"
          ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item  :label="isMobile ? '' : '价位'" prop="price">
        <el-col :span="11">
          <el-input v-model="form.fromPrice" placeholder="最低价" :maxlength="8"></el-input>
        </el-col>
        <el-col class="line" :span="2">至</el-col>
        <el-col :span="11">
          <el-input v-model="form.toPrice" placeholder="最高价" :maxlength="8"></el-input>
        </el-col>
      </el-form-item>
      <el-form-item  :label="isMobile ? '' : '房源'">
        <el-select v-model="form.source" placeholder="请选择房源" style="width: 100%" filterable>
          <el-option label="全部" value=""></el-option>
          <el-option
              v-for="item in source"
              :label="item.label"
              :value="item.value"
              :key="item.value"
          >
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item  :label="isMobile ? '' : '时限(天数)'" prop="intervalDay">
        <el-input v-model="form.intervalDay" placeholder="几天内的数据？默认十天" :maxlength="8"></el-input>
      </el-form-item>
      <el-form-item :label="isMobile ? '' : '关键词'">
        <el-input v-model="form.keyword" placeholder="搜索关键字" :maxlength="50"></el-input>
      </el-form-item>
      <el-form-item :label="isMobile ? '' : '数据展示方式'" prop="type">
        <el-select v-model="form.type" placeholder="请选择数据展示方式" style="width: 100%">
          <el-option
              label="列表"
              value="0"
          ></el-option>
          <el-option
              label="地图"
              value="1"
          ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label-width="0px">
        <el-collapse-transition>
          <el-alert
              v-show="loading"
              style="line-height: normal"
              center
              title="首次搜索数据可能较慢,请耐心等待"
              type="info">
          </el-alert>
        </el-collapse-transition>
        <el-button type="primary" @click="search" :loading="loading" class="search">开始搜索</el-button>
      </el-form-item>
    </el-form>
  </el-dialog>
</template>
<style lang="scss" scoped>
  .form {
    text-align: center;
  }

  .search {
    margin-top: 10px;
  }
</style>
<script>
  import validate from './../validate/index';
  import houseSearchList from './../components/house-search-list'

  export default {
    components: {
      houseSearchList
    },
    props: {
      visible: {
        default: true
      },
      isMobile: {},
      token: {}
    },
    data() {
      return {
        form: {
          cityName: '上海',
          intervalDay: 14,
          source: '',
          type: '0',
        },
        currentPage: 1,
        searchRes: false,
        loading: false,
        rules: (() => {

          const isNum = (rule, value, callback) => {

            if (this.form.fromPrice || this.form.toPrice) {
              const re = /^[0-9]+.?[0-9]*$/;
              if (this.form.fromPrice && (!re.test(this.form.fromPrice)) || (this.form.toPrice && !re.test(this.form.toPrice))) {
                callback(new Error())
              } else {
                callback()
              }
            } else {
              callback()
            }
          };
          return {
            cityName: [
              {required: true, message: '请选择地区', trigger: 'change'},
            ],
            price: [
              {message: '价位只能是正数', type: 'number', validator: isNum}
            ],
            intervalDay: [
              {message: '时限只能是正数', type: 'number', validator: validate.isNum},
              {message: '时限只能是正整数', type: 'number', validator: validate.isInteger},
            ],
            type: [
              {required: true, message: '请选择展示方式', trigger: 'change'},
            ],
          }
        })(),
        source: [
          {
            label: '豆瓣租房小组',
            value: 'douban'
          },
          {
            label: 'Zuber合租平台',
            value: 'zuber'
          },
          {
            label: '蘑菇租房',
            value: 'mogu'
          },
          {
            label: 'CCB建融家园',
            value: 'ccbhouse'
          },
          {
            label: '上海互助租房',
            value: 'huzhuzufang'
          },
          {
            label: '58同城品牌公寓',
            value: 'pinpaigongyu'
          }
        ],
        cities: [],
        houseList: []
      }
    },
    methods: {
      closeSearchList() {
        this.searchRes = false;
        this.$refs['search-list'].reset();
      },
      close() {
        this.$emit('close', 'searchVisible', false)
      },
      async search() {
        try {
          await this.$refs.form.validate();
          const params = Object.assign({}, this.form);
          delete params.type;
          if (this.form.type == 1) {
            params.cityname = params.cityName;
            params.token = this.token;
            delete params.cityName;
            window.open(`https://www.woyaozufang.live/Home/HouseList?${this.$qs.stringify(params)}`);
          } else {
            this.getHousesList(params)
          }
        } catch (e) {
          this.loading = false;
        }
      },
      async getHousesList(options,type) {
        const params = Object.assign({
          houseCount: 100,
          page:1
        }, options);
        if(!type) {
          this.loading = true;
        }
        const data = await this.$ajax.post('/houses', {
          ...params
        });
        if(!type) {
          this.loading = false;
          this.houseList = data.data;
          this.searchRes = true;
        }else {
          this.houseList = data.data;
        }
      },
      async getCities() {
        const data = await this.$ajax.get('/houses/citys');
        this.cities = data.data;
      }
    },
    created() {
      this.getCities()
    }
  }
</script>