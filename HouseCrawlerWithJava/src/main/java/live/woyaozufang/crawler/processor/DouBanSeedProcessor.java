package live.woyaozufang.crawler.processor;

import com.virjar.sipsoup.parse.XpathParser;
import com.virjar.vscrawler.core.net.session.CrawlerSession;
import com.virjar.vscrawler.core.processor.CrawlResult;
import com.virjar.vscrawler.core.processor.SeedProcessor;
import com.virjar.vscrawler.core.seed.Seed;
import org.apache.catalina.LifecycleState;
import org.apache.commons.lang3.StringUtils;
import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;
import org.jsoup.select.Elements;

import java.util.List;
import java.util.logging.Logger;

public class DouBanSeedProcessor implements SeedProcessor {

    public void process(final Seed seed, final CrawlerSession crawlerSession, final CrawlResult crawlResult) {
        String html = crawlerSession.getCrawlerHttpClient().get(seed.getData());
        if (html == null) {
            seed.retry();
            return;
        }
        Document doc = Jsoup.parse(html);
        Elements elements = doc.getElementsByClass("title");
        for (Element element:elements){
            System.out.println(element.text());
        }

    }
}
