<template>
  <el-dialog
      title="高级搜索功能"
      :visible.sync="visible"
      :width="isMobile ? '100%' : '550px'"
      center
      :before-close="close"
  >
    <component v-if="view" :is="view"></component>
    <el-dialog
        top="50px"
        :width="isMobile ? '100%' : '70%'"
        title="房源"
        :visible.sync="searchRes"
        append-to-body
        center
        :before-close="closeSearchList"
    >
      <house-search-list type="all" :house-list="houseList" :options="form" :get-houses-list="getHousesList"
                         key="all"
                         ref="search-list"></house-search-list>
    </el-dialog>
    <el-form ref="form" :model="form" :label-width="isMobile ? '0px' : '130px'" class="form" :rules="rules">
      <el-form-item :label="isMobile ? '' : '地区'" prop="city">
        <el-select v-model="form.city" placeholder="请选择地区" style="width: 100%" @change="cityChange" filterable
                   allow-create>
          <!--<el-option label="全部" value=""></el-option>-->
          <el-option
              v-for="item in cities"
              :key="item.id"
              :label="item.city"
              :value="item.city"
          ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item :label="isMobile ? '' : '价位'" prop="price">
        <el-col :span="11">
          <el-input v-model="form.fromPrice" placeholder="最低价" :maxlength="8"></el-input>
        </el-col>
        <el-col class="line" :span="2">至</el-col>
        <el-col :span="11">
          <el-input v-model="form.toPrice" placeholder="最高价" :maxlength="8"></el-input>
        </el-col>
      </el-form-item>
      <el-form-item :label="isMobile ? '' : '房源'">
        <el-select v-model="form.source" placeholder="请选择房源" style="width: 100%" filterable>
          <el-option label="全部" value=""></el-option>
          <el-option
              v-for="item in source"
              :label="item.displaySource"
              :value="item.source"
              :key="item.id"
          >
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item :label="isMobile ? '' : '时限(天数)'" prop="intervalDay">
        <el-input v-model="form.intervalDay" placeholder="几天内的数据？默认十天" :maxlength="8"></el-input>
      </el-form-item>
      <el-form-item :label="isMobile ? '' : '关键字'">
       <div class="keyword-tag-wrap">
        <div class="keyword-tag">
          <el-tag
              class="tag"
              :key="tag"
              v-for="tag in keywordArr"
              closable
              :disable-transitions="false"
              @close="removeKeyword(tag)">
            {{tag}}
          </el-tag>
        </div>
         <el-input
             class="keyword-tag-input"
             v-model="keywordTag"
             ref="saveTagInput"
             size="small"
             v-if="inputVisible"
             @keyup.enter.native="keywordConfirm"
             @blur="keywordConfirm"
             placeholder="搜索关键字"
             :maxlength="50"></el-input>
         <el-button v-else class="button-new-tag" size="small" @click="showInput">+ 关键词</el-button>
       </div>


      </el-form-item>
      <el-form-item :label="isMobile ? '' : '房源类型'" prop="rentType">
        <el-select v-model="form.rentType" placeholder="请选择房源类型" style="width: 100%">
          <el-option
              :ket="item.value"
              v-for="item in rentTypeArr"
              :label="item.label"
              :value="item.value"
          ></el-option>
        </el-select>
      </el-form-item>
      <!--<el-form-item :label="isMobile ? '' : '数据展示方式'" prop="type">-->
        <!--<el-select v-model="form.type" placeholder="请选择数据展示方式" style="width: 100%">-->
          <!--<el-option-->
              <!--label="列表"-->
              <!--value="0"-->
          <!--&gt;</el-option>-->
          <!--&lt;!&ndash;<el-option&ndash;&gt;-->
          <!--&lt;!&ndash;label="地图"&ndash;&gt;-->
          <!--&lt;!&ndash;value="1"&ndash;&gt;-->
          <!--&lt;!&ndash;&gt;</el-option>&ndash;&gt;-->
        <!--</el-select>-->
      <!--</el-form-item>-->
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
        <el-button @click="toMap" class="search">直达地图</el-button>
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
  .keyword-tag-wrap{
    display: flex;
    align-items: center;
    min-height: 40px;
  }
  .keyword-tag{
    text-align: left;
    .tag{
      margin-right: 10px;
    }
  }
  .keyword-tag-input{
    max-width: 100px;
  }
</style>
<script>
  import validate from './../validate/index';
  import houseSearchList from './../components/house-search-list'

  const asyncComponent = require('./../components/async-component.js').default;
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
        view: undefined,
        form: {
          city: '上海',
          intervalDay: 14,
          source: '',
          type: '0',
          rentType:undefined
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
            city: [
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
        houseList: [],
        rentTypeArr:[
          {
            label:'全部',
            value:undefined
          },
          {
            label:'未知',
            value:0
          },
          {
            label:'合租',
            value:1
          },
          {
            label:'单间',
            value:2
          },
          {
            label:'整套出租',
            value:3
          },
          {
            label:'公寓',
            value:4
          },
        ],
        keywordArr:[],
        inputVisible:false,
        keywordTag:''
      }
    },
    methods: {
      showInput() {
        this.inputVisible = true;
        this.$nextTick(_ => {
          this.$refs.saveTagInput.$refs.input.focus();
        });
      },
      keywordConfirm() {
        let inputValue = this.keywordTag;
        if (inputValue) {
          let has = this.keywordArr.find(item=>{
            return item === inputValue
          });
          if(!has) {
            this.keywordArr.push(inputValue);
          }
        }
        this.inputVisible = false;
        this.keywordTag = '';
      },
      removeKeyword(item) {
        this.keywordArr.splice(this.keywordArr.indexOf(item), 1);
      },
      closeSearchList() {
        this.searchRes = false;
        this.$refs['search-list'].reset();
      },
      close() {
        this.$emit('close', 'searchVisible', false)
      },
      async cityChange(cityName) {
        const data = await this.$v2.get(`/cities/${cityName}`);
        this.source = data.data;
        this.form.source = '';
      },
      async toMap() {
        try {

          await this.$refs.form.validate();
          const params = Object.assign({}, this.form);
          delete params.type;
          if (this.keywordArr.length) {
            params.keyword =  this.keywordArr.join(',');
            this.form.keyword = params.keyword;
          }
          // params.cityname = params.cityName;
          // // params.token = this.token;
          // delete params.cityName;
          // window.open(`https://api.house-map.cn/Home/HouseList?${this.$qs.stringify(params)}`);
          let {href} = this.$router.resolve({path: `/Map?${this.$qs.stringify(params)}`});
          window.open(href, '_blank');
        } catch (e) {
          this.loading = false;
        }
      },
      async search() {
        try {
          await this.$refs.form.validate();
          let params = Object.assign({}, this.form);
          delete params.type;
          // params.city = params.cityName;
          // delete params.cityName;

          if (this.keywordArr.length) {
            params.keyword =  this.keywordArr.join(',');
            this.form.keyword = params.keyword;
          }
          if (gtag) {
            gtag('event', '高级搜索', {
              'event_category' : this.form.city
            });
          }

          this.getHousesList(params)
        } catch (e) {
          this.loading = false;
          console.log(e)
        }
      },
      async getHousesList(options, type) {
        let params = Object.assign({
          size: 100,
          page: 0
        }, options);
        if (!type) {
          this.loading = true;
        }
        // if (!params.city) {
        //   params.city = params.cityName;
        // }
        // delete params.cityName;
        const data = await this.$v2.get(`/houses?${this.$qs.stringify(params)}`);
        // const data = await this.$v2.post(`/houses`,params);

        if (this.isMobile) {
          let com = require('./../components/mobile/house-list').default;
          try {
            await asyncComponent(com, {
              props: {
                list: data.data,
                appendToBody: true,
                params
              }
            }, (template) => {
              this.view = template;
              this.loading = false;

            });
            this.view = undefined;
          } catch (e) {
            this.view = undefined;
          }
        } else {
          if (!type) {
            this.loading = false;
            this.houseList = data.data;
            this.searchRes = true;
          } else {
            this.houseList = data.data;
          }
        }

      },
      async getCities() {
        const data = await this.$v2.get('/cities?fields=id,city');
        this.cities = data.data;
        this.cityChange(this.form.city)
      }
    },
    created() {
      this.getCities()
    }
  }
</script>