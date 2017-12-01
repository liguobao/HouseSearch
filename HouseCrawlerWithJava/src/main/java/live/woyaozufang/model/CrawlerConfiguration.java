package live.woyaozufang.model;

public class CrawlerConfiguration {
    private Long id;
    private String configurationName;
    private String configurationKey;
    private String configurationValue;
    private Integer enabled;

    public Long getId() {
        return id;
    }

    public void setId(final Long id) {
        this.id = id;
    }

    public String getConfigurationName() {
        return configurationName;
    }

    public void setConfigurationName(final String configurationName) {
        this.configurationName = configurationName;
    }

    public String getConfigurationKey() {
        return configurationKey;
    }

    public void setConfigurationKey(final String configurationKey) {
        this.configurationKey = configurationKey;
    }

    public String getConfigurationValue() {
        return configurationValue;
    }

    public void setConfigurationValue(final String configurationValue) {
        this.configurationValue = configurationValue;
    }

    public Integer getEnabled() {
        return enabled;
    }

    public void setEnabled(final Integer enabled) {
        this.enabled = enabled;
    }
}
