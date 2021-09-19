(defproject ucdump "0.2.0-SNAPSHOT"
  :description "Unicode character dump for UTF-8 encoded files"
  :url "https://github.com/jerekapyaho/ucdump"
  :license {:name "MIT License"
            :url "http://opensource.org/licenses/MIT"}
  :dependencies [[org.clojure/clojure "1.10.0"]]
  :main ^:skip-aot com.coniferproductions.ucdump
  :target-path "target/%s"
  :profiles {:uberjar {:aot :all}})
