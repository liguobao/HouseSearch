package live.woyaozufang.crawler.processor;

import com.virjar.sipsoup.parse.XpathParser;
import com.virjar.vscrawler.core.net.session.CrawlerSession;
import com.virjar.vscrawler.core.processor.CrawlResult;
import com.virjar.vscrawler.core.processor.SeedProcessor;
import com.virjar.vscrawler.core.seed.Seed;
import org.apache.commons.lang3.StringUtils;
import org.jsoup.Jsoup;

public class DouBanSeedProcessor implements SeedProcessor {
    public void process(final Seed seed, final CrawlerSession crawlerSession, final CrawlResult crawlResult) {

        if (StringUtils.endsWithIgnoreCase(seed.getData(), ".jpg")) {
            //handlePic(seed, crawlerSession);
        } else {
            String s = crawlerSession.getCrawlerHttpClient().get(seed.getData());
            if (s == null) {
                seed.retry();
                return;
            }
            // 将下一页的链接和图片链接抽取出来
            crawlResult.addStrSeeds(XpathParser
                    .compileNoError(
                            "/css('#pages a')::self()[contains(text(),'下一页')]/absUrl('href') | /css('.content')::center/img/@src")
                    .evaluateToString(Jsoup.parse(s, seed.getData())));
        }
    }
}
