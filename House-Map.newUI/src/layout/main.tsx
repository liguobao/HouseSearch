import { Box } from "@mui/material";
import {
  Radio,
  Button,
  Grid,
  Form,
  Input,
  Select,
  List,
  Menu,
  Dropdown,
  Modal,
  Carousel,
  Spin,
} from "@arco-design/web-react";
import { useNavigate } from "react-router-dom";
import { FC, useEffect, useState } from "react";
import { IconMenu } from "@arco-design/web-react/icon";
import styles from "./main.module.scss";
import { getHouseList } from "../services/house";
import HouseCard from "../components/house-card";
import { isMobile } from "../utils/base";
interface searchParamsProps {
  city?: string | null;
  source?: string | null;
  rentType?: number | null;
  fromPrice?: number | null;
  toPrice?: number | null;
  page?: number | null;
  pageSize?: number | null;
}
interface houseProps {
  id?: string;
  city: string;
  source: string;
  rentType?: number;
  fromPrice?: number;
  toPrice?: number;
  page: number;
  pageSize: number;
  pictures?: string[];
  publishDate?: string;
  displayRentType?: string;
  displaySource?: string;
  district?: string;
  location?: string;
  onlineURL?: string;
  price?: string;
}
const Main: FC = () => {
  const Row = Grid.Row;
  const Col = Grid.Col;
  const FormItem = Form.Item;
  const mobile = isMobile();
  const history = useNavigate();
  const [list, setList] = useState<houseProps[]>([]); // 房源列表展示的数据
  const [totalData, setTotalData] = useState<houseProps[]>([]); // 总数据
  const [visible, setVisible] = useState(false); // 是否显示相信数据的对话框
  const [selectedHouseData, setSelectedHouseData] = useState<houseProps>(); // 选中的房源数据
  //const [form] = Form.useForm();
  const [searchParams, setSearchParams] = useState<searchParamsProps>({
    city: "上海",
    source: "",
    page: 1,
    pageSize: 50,
  }); // 搜索参数
  const [scrollLoading, setScrollLoading] = useState(<Spin loading={true} />);
  const checkList = [
    {
      label: "全部",
      value: "",
    },
    {
      label: "Zuber",
      value: "zuber",
    },
    {
      label: "随申办",
      value: "shgovrent",
    },
    {
      label: "豆瓣",
      value: "douban",
    },
    {
      label: "蘑菇租房",
      value: "mogu",
    },
    {
      label: "贝壳",
      value: "beike",
    },
    {
      label: "互助租房",
      value: "shanghaihuzhu",
    },
    {
      label: "安居客",
      value: "anjuke",
    },
    {
      label: "城家",
      value: "cjia",
    },
    {
      label: "我爱我家",
      value: "woaiwojia",
    },
    {
      label: "唯心所寓",
      value: "wellcee",
    },
    {
      label: "xhs",
      value: "xhs",
    },
    {
      label: "巴乐兔",
      value: "baletu",
    },
  ];
  const houseTypeOptions = [
    { label: "全部", value: null },
    { label: "未知", value: 0 },
    { label: "合租", value: 1 },
    { label: "整租", value: 2 },
    { label: "单间", value: 3 },
    { label: "公寓", value: 4 },
  ];
  //展示选中的房源数据弹窗
  const showSelectedHouseDataModal = (data: any) => {
    if (mobile) {
      // 如果是移动端，跳转到详情页
      history(`/house/${data.id}`);
    } else {
      // 如果不是移动端，通过modal的方式打开
      setSelectedHouseData(data);
      setVisible(true);
    }
  };
  const loadDataFrom = async (key: any, value: any) => {
    const res = await getHouseList({
      city: searchParams.city || null,
      source: searchParams.source || null,
      rentType: searchParams.rentType || null,
      fromPrice: searchParams.fromPrice || null,
      toPrice: searchParams.toPrice || null,
      [key]: value,
    });
    setTotalData(res.data);
    // 清空当前的 list
    setList([]);

    // 从新的总数据中加载第一页的数据
    const firstPageData = res.data.slice(0, searchParams.pageSize);
    setList(firstPageData);
    setSearchParams({
      ...searchParams,
      page: 1,
      [key]: value,
    });
  };
  const loadMore = () => {
    var page = searchParams?.page ?? 0;
    var pageSize = searchParams?.pageSize ?? 20;
    const nextPageData = totalData.slice(
      page * pageSize,
      (page + 1) * pageSize
    );
    setList(list.concat(nextPageData));
    // 如果没有更多的数据可以加载，那么停止加载
    if ((page + 1) * pageSize >= totalData.length) {
      setScrollLoading("没有更多数据了" as any);
    }

    // 页数加一
    setSearchParams({
      ...searchParams,
      page: page + 1,
    });
  };
  const onFinish = async (values: searchParamsProps) => {
    console.log("values", values);
    const res = await getHouseList({
      city: values.city || searchParams.city || null,
      source: values.source || searchParams.source || null,
      rentType: values.rentType || searchParams.rentType || null,
      fromPrice: values.fromPrice || searchParams.fromPrice || null,
      toPrice: values.toPrice || searchParams.toPrice || null,
    });
    setTotalData(res.data);
    // 清空当前的 list
    setList([]);
    //给list赋值
    const firstPageData = res.data.slice(0, searchParams.pageSize);
    setList(firstPageData);
    //给searchParams赋值
    setSearchParams({
      ...searchParams,
      city: values.city || searchParams.city || null,
      source: values.source || searchParams.source || null,
      rentType: values.rentType || searchParams.rentType || null,
      fromPrice: values.fromPrice || searchParams.fromPrice || null,
      toPrice: values.toPrice || searchParams.toPrice || null,
      page: 1,
    });
  };
  useEffect(() => {
    const getList = async () => {
      const res = await getHouseList(searchParams);
      setTotalData(res.data);
    };

    getList();
  }, []); // 注意这里的空数组

  useEffect(() => {
    loadMore();
  }, [totalData]);
  const dropList = (
    <Menu>
      <Menu.Item
        key="1"
        onClick={() =>
          window.open("https://wj.qq.com/s/2953926/aabe", "_blank")
        }
      >
        帮我们做的更好
      </Menu.Item>
      <Menu.Item key="2">个人中心</Menu.Item>
      <Menu.Item key="3">说明/公告</Menu.Item>
      <Menu.Item key="4">关于我们</Menu.Item>
    </Menu>
  );
  return (
    <Box className={styles.container}>
      <Modal
        title=""
        visible={visible}
        onOk={() => setVisible(false)}
        onCancel={() => setVisible(false)}
        autoFocus={false}
        focusLock={true}
        footer={null}
        style={{ width: "900px", height: "650px" }}
      >
        <div style={{ display: "flex", height: "700px" }}>
          <div style={{ width: "50%", height: "100%" }}>
            <Carousel style={{ height: "85%" }}>
              {selectedHouseData && selectedHouseData.pictures &&
                selectedHouseData.pictures.map((src, index) => (
                  <div key={index}>
                    <img src={src} style={{ width: "100%", height: "100%" }} />
                  </div>
                ))}
            </Carousel>
          </div>
          <div
            style={{
              display: "flex",
              flexDirection: "column",
              alignItems: "center",
              width: "50%",
              padding: "20px",
            }}
          >
            <h2>
              {selectedHouseData && selectedHouseData.price == "-1"
                ? "暂无房价"
                : selectedHouseData && selectedHouseData.price}
            </h2>
            <div
              style={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                justifyContent: "space-between",
                height: "40%",
              }}
            >
              <p>城市: {selectedHouseData?.city}</p>
              <p>发布日期: {selectedHouseData?.publishDate}</p>
              <p>租赁类型: {selectedHouseData?.displayRentType}</p>
              <p>来源: {selectedHouseData?.displaySource}</p>
              <p>地区: {selectedHouseData?.district}</p>
              <p>位置: {selectedHouseData?.location}</p>
              <p>
                <a href={selectedHouseData?.onlineURL}>在线链接</a>
              </p>
            </div>
          </div>
        </div>
      </Modal>
      <Row className={styles.header}>
        <Col span={mobile ? 18 : 10}>
          <a
            className={styles["logo-content"]}
            href="https://wj.qq.com/s/2953926/aabe"
            target="_blank"
          >
            地图搜租房
          </a>
        </Col>
        <Col className={styles["channel-btn"]} span={6}>
          <Button
            type="text"
            onClick={() =>
              window.open("https://wj.qq.com/s/2953926/aabe", "_blank")
            }
          >
            帮我们做的更好
          </Button>
          <Button type="text">个人中心</Button>
          <Button type="text">说明/公告</Button>
          <Button type="text">关于我们</Button>
        </Col>
        <Col className={styles["channel-btn-mobile"]} span={4}>
          <Dropdown
            droplist={dropList}
            position="br"
            // style={{ width: "70px", height: "70px" }} 
          >
            <Button type="text" icon={<IconMenu />} size="large"></Button>
          </Dropdown>
        </Col>
      </Row>
      <Box className={styles.center}>
        <Box className={styles.left}>
          <Box className={styles["search-table"]}>
            <Form onSubmit={onFinish}>
              <FormItem label="类型" field="rentType">
                <Select placeholder="请选择类型" 
                options={houseTypeOptions as any} />
              </FormItem>
              <FormItem label="价格区间">
                <Grid.Row gutter={8}>
                  <Grid.Col span={12}>
                    <FormItem field="fromPrice">
                      <Input />
                    </FormItem>
                  </Grid.Col>
                  <Grid.Col span={12}>
                    <FormItem field="toPrice">
                      <Input />
                    </FormItem>
                  </Grid.Col>
                </Grid.Row>
              </FormItem>
              <FormItem label="地区" field="city">
                <Input placeholder="请输入地区" />
              </FormItem>
              <Col style={{ display: "flex", justifyContent: "center" }}>
                <Button
                  size="large"
                  className={styles["search-button"]}
                  type="primary"
                  htmlType="submit"
                >
                  搜索
                </Button>
              </Col>
            </Form>
          </Box>
        </Box>
        <Box className={styles.right}>
          <Row className={styles["search-title"]}>
            <Radio.Group
              defaultValue={"全部"}
              name="button-radio-group"
              style={{
                display: "flex",
                overflowX: "scroll",
                whiteSpace: "nowrap",
                scrollbarWidth: "none",
                msOverflowStyle: "none",
                WebkitOverflowScrolling: "touch",
              }}
            >
              {checkList.map((item) => {
                return (
                  <Radio key={item.value} value={item.value}>
                    {({ checked }) => {
                      return (
                        <Button
                          tabIndex={-1}
                          key={item.label}
                          shape="round"
                          type={checked ? "primary" : "default"}
                          onClick={() => loadDataFrom("source", item.value)}
                        >
                          {item.label}
                        </Button>
                      );
                    }}
                  </Radio>
                );
              })}
            </Radio.Group>
          </Row>
          <List
            className={styles["card-list"]}
            onReachBottom={loadMore}
            style={{ maxHeight: "800px", overflow: "auto" }}
            grid={{
              sm: 12,
              md: 12,
              lg: 8,
              xl: 6,
              xs: 12,
            }}
            dataSource={list}
            scrollLoading={scrollLoading}
            bordered={false}
            render={(item, index) => (
              <List.Item
                key={index}
                onClick={() => showSelectedHouseDataModal(item)}
              >
                <HouseCard 
                className={styles.card as any} 
                {...item} key={item.id} />
              </List.Item>
            )}
          />
        </Box>
      </Box>
    </Box>
  );
};

export default Main;
