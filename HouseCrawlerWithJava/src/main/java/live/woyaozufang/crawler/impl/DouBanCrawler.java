package live.woyaozufang.crawler.impl;

import com.google.gson.Gson;
import com.virjar.vscrawler.core.VSCrawler;
import com.virjar.vscrawler.core.VSCrawlerBuilder;
import live.woyaozufang.common.DouBanConfig;
import live.woyaozufang.crawler.EmptyPipeline;
import live.woyaozufang.crawler.ICrawler;
import live.woyaozufang.crawler.processor.DouBanSeedProcessor;
import live.woyaozufang.mapper.CrawlerConfigurationMapper;
import live.woyaozufang.model.CrawlerConfiguration;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class DouBanCrawler implements ICrawler {

    @Autowired
    private CrawlerConfigurationMapper configurationMapper;

    @Scheduled(fixedRate = 5000)
    public void run() {
        List<CrawlerConfiguration> configurations = configurationMapper.findByName("douban");
        if (configurations != null) {
            DouBanSeedProcessor douBanSeedProcessor = new DouBanSeedProcessor();
            VSCrawler vsCrawler = VSCrawlerBuilder.create().addPipeline(new EmptyPipeline())
                    .setProcessor(douBanSeedProcessor).build();
            // 清空历史爬去数据,或者会断点续爬
            vsCrawler.clearTask();
            String preDoubanURL = "https://www.douban.com/group/%s/discussion?start=%d";
            Gson gson = new Gson();
            for (CrawlerConfiguration configuration : configurations) {
                DouBanConfig douBanConfig = gson.fromJson(configuration.getConfigurationValue(), DouBanConfig.class);
                if (douBanConfig != null) {
                    for (Integer index = 0; index <= douBanConfig.getPagecount(); index++) {
                        String doubanURL = String.format(preDoubanURL, douBanConfig.getGroupid(), index * 25);
                        vsCrawler.pushSeed(doubanURL);
                    }
                }
            }
            vsCrawler.stopCrawler();
        }
    }
}
