<template>
  <div>
    <el-table
        :data="list"
        border
        v-loading="loading"
        max-height="800"
        :row-key="rowKey"
        :expand-row-keys="rowKeyArr"
        @cell-click="cellClick"
        style="width: 100%">
      <el-table-column
          type="expand"
          width="80"
          label="图片">
        <template slot-scope="props">
          <div class="carousel">
            <el-carousel trigger="click" height="150px" :autoplay="false"
                         v-if="props.row.pictures && props.row.pictures.length" arrow="always">
              <el-carousel-item v-for="item in props.row.pictures" :key="item">
                <div class="carousel-image">
                  <transition name="el-fade-in">
                    <i v-show="!imagesLoadingMap[item]" class="el-icon-loading loading-icon"></i>
                  </transition>
                  <transition name="el-fade-in">
                    <a target="_blank" :style="{'background-image': `url(${item})`}" v-show="imagesLoadingMap[item]"
                       :href="item" title="查看原图"><img :src="item"
                                                      @load="imageLoading(item,imagesLoadingMap)"/></a>
                  </transition>
                </div>
              </el-carousel-item>
            </el-carousel>
            <span v-else>暂无图片</span>
          </div>
        </template>
      </el-table-column>
      <el-table-column
          align="center"
          prop="locationCityName"
          label="城市"
          width="80">
      </el-table-column>
      <el-table-column
          align="center"
          prop="houseTitle"
          label="标题"
          width="300">
        <template slot-scope="scope">
          <el-popover
              placement="top-start"
              title=""
              width="300"
              trigger="hover"
              :content="scope.row.houseTitle">
            <a slot="reference" class="ellipsis text-left link" :href="scope.row.houseOnlineURL" target="_blank">{{
              scope.row.houseTitle }}</a>
          </el-popover>
        </template>
      </el-table-column>
      <el-table-column
          align="center"
          prop="houseLocation"
          width="150"
          label="坐标">
        <template slot-scope="scope">
          <el-popover
              placement="top-start"
              title=""
              width="300"
              trigger="hover"
              :content="scope.row.houseLocation">
            <span slot="reference" class="ellipsis text-left">{{ scope.row.houseLocation }}</span>
          </el-popover>
        </template>
      </el-table-column>
      <el-table-column
          align="center"
          width="150"
          prop="housePrice"
          label="价格(元)">
      </el-table-column>
      <el-table-column
          align="center"
          prop="pubTime"
          width="180"
          label="发布时间">
        <template slot-scope="scope">
          <i class="el-icon-time"></i> <span class="pub-time">{{ scope.row.pubTime }}</span>
        </template>
      </el-table-column>
      <el-table-column
          align="center"
          prop="displaySource"
          label="来源">
      </el-table-column>
      <el-table-column
          v-if="type === 'user'"
          width="120"
          align="center"
          label="操作">
        <template slot-scope="scope">
          <el-button type="danger" size="small" @click="del(scope.row,scope.$index)" :loading="loading">删除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <div class="pagination text-right" v-if="type === 'all' && !loading">
      <el-pagination
          @current-change="currentChange"
          :current-page="currentPage"
          :page-size="100"
          layout="prev, pager, next"
          :total="2000">
      </el-pagination>
    </div>
  </div>
</template>
<style scoped lang="scss">
  .link {
    color: #409EFF;
  }

  .pub-time {
    margin-left: 5px;
  }

  .carousel {
    max-width: 75%;
    .carousel-image {
      display: flex;
      align-items: center;
      justify-content: center;
      height: 100%;
      text-align: center;
      position: relative;
      a {
        display: block;
        width: 100%;
        height: 100%;
        text-align: center;
        background-size: contain;
        background-position: center;
        background-repeat: no-repeat;
      }
      img {
        display: none;
        max-width: 100%;
        margin: auto;
      }
    }
    .loading-icon {
      color: #409EFF;
      position: absolute;
      left: 50%;
      top: 50%;
      z-index: 26;
      transform: translate(-50%, -50%);

    }
  }
  .pagination{
    margin-top: 20px;
  }
</style>
<script>
  export default {
    props: {
      type: {
        default: 'all'
      },
      options: {},
      token: {},
      houseList: {
        default: () => {
          return []
        }
      },
      getHousesList: {}
    },
    watch: {
      houseList(n) {
        if (n) {
          this.list = this.houseList
        }
      }
    },
    data() {
      return {
        imagesLoadingMap: {},
        loading: false,
        list: this.houseList,
        rowKeyArr: [],
        currentPage: 1
      }
    },
    methods: {
      reset() {
        this.currentPage = 1;
        this.rowKeyArr = [];
      },
      async currentChange(page) {
        this.loading = true;
        await this.getHousesList({
          ...this.options,
          page
        },'change');
        this.loading = false;
        this.currentPage = page;
      },
      cellClick(row, column, cell) {
        if (cell.cellIndex !== 2) {
          const id = ((row.id + '-') + row.source);
          const index = this.rowKeyArr.findIndex(item => {
            return item === id
          });
          if (index >= 0) {
            this.rowKeyArr.splice(index, 1)
          } else {
            this.rowKeyArr.push(id);
          }
        }
      },
      rowKey(row) {
        return (row.id + '-') + row.source
      },
      async del(row, index) {
        this.loading = true;
        const userId = this.$store.state.userInfo.id;
        const data = await this.$ajax.delete(`/users/${userId}/collections/${row.id}`);
        this.list.splice(index, 1);
        this.loading = false;
        this.$message.success(data.message ? data.message : '删除成功')
      }
    },
    created() {

    }
  }
</script>