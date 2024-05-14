"use strict";

(function (scope) {
	"use strict";

	var styleFunction = function styleFunction(feature) {

		//Map: Color coding: color plot differently by contributing networks
		//red: AfriTRON, RAINFOR, TForces
		//green: ForestGEO
		//blue: TmFO
		//yellow: IIASA
		//black: Other

		var color;

		if (feature.network.trim().toLowerCase() == "afritron") {
			color = "red";
		} else if (feature.network.trim().toLowerCase() == "rainfor") {
			color = "red";
		} else if (feature.network.trim().toLowerCase() == "tforces") {
			color = "red";
		} else if (feature.network.trim().toLowerCase() == "forestgeo") {
			color = "green";
		} else if (feature.network.trim().toLowerCase() == "tmfo") {
			color = "blue";
		} else if (feature.network.trim().toLowerCase() == "iiasa") {
			color = "peru";
		} else {
			color = "black";
		}

		var retStyle = new ol.style.Style({
			image: new ol.style.Circle({
				radius: 4,
				fill: new ol.style.Fill({
					color: "#cccccc"
				}),
				stroke: new ol.style.Stroke({
					color: color,
					width: 1
				})
			})
		});
		return retStyle;
	};

	var styleFunction_polygons = function styleFunction_polygons(feature) {

		//Map: Color coding: color plot differently by contributing networks
		//red: AfriTRON, RAINFOR, TForces
		//green: ForestGEO
		//blue: TmFO
		//yellow: IIASA
		//black: Other

		var color;

		if (feature.network.trim().toLowerCase() == "afritron") {
			color = "red";
		} else if (feature.network.trim().toLowerCase() == "rainfor") {
			color = "red";
		} else if (feature.network.trim().toLowerCase() == "tforces") {
			color = "red";
		} else if (feature.network.trim().toLowerCase() == "forestgeo") {
			color = "green";
		} else if (feature.network.trim().toLowerCase() == "tmfo") {
			color = "blue";
		} else if (feature.network.trim().toLowerCase() == "iiasa") {
			color = "peru";
		} else {
			color = "black";
		}

		var retStyle = new ol.style.Style({
			stroke: new ol.style.Stroke({
				color: color,
				lineDash: [4],
				width: 3
			}),
			fill: new ol.style.Fill({
				color: 'rgba(0, 0, 255, 0.1)'
			})
		});
		return retStyle;
	};

	var styleFunction_clustered = function styleFunction_clustered(feature) {

		//Map: Color coding: color plot differently by contributing networks
		//red: AfriTRON, RAINFOR, TForces
		//green: ForestGEO
		//blue: TmFO
		//yellow: IIASA
		//black: Other
		var styleCache_plotFeature = {};
		var color = "black";
		var size = feature.get('features').length;
		var style = styleCache_plotFeature[size];

		if (size > 0) {

			var first = feature.get('features')[0];
			if (first.network.trim().toLowerCase() == "afritron") {
				color = "red";
			} else if (first.network.trim().toLowerCase() == "rainfor") {
				color = "red";
			} else if (first.network.trim().toLowerCase() == "tforces") {
				color = "red";
			} else if (first.network.trim().toLowerCase() == "forestgeo") {
				color = "green";
			} else if (first.network.trim().toLowerCase() == "tmfo") {
				color = "blue";
			} else if (first.network.trim().toLowerCase() == "iiasa") {
				color = "peru";
			} else {
				color = "black";
			}
		}

		if (!style) {
			style = new ol.style.Style({
				image: new ol.style.Circle({
					radius: 10,
					stroke: new ol.style.Stroke({
						color: '#fff'
					}),
					fill: new ol.style.Fill({
						color: color
					})
				}),
				text: new ol.style.Text({
					text: size.toString(),
					fill: new ol.style.Fill({
						color: '#fff'
					})
				})
			});
			styleCache_plotFeature[size] = style;
		}
		return style;
	};
	scope.map = (function () {

		var self = {};

		var attribution = new ol.Attribution({
			html: "Tiles &copy; <a href=\"https://earthenginepartners.appspot.com/science-2013-global-forest\">Global Forest Change (Hansen et. al.)</a>"
		});

		self.viewModel = {
			loading: ko.observable(false),
			title: ko.observable(""),
			siteDetails: ko.observable(""),
			plotDetails: ko.observable(""),
			photoUrl: ko.observable(""),
			link: ko.observable("")
		};

		var IsNullOrUndefined = function IsNullOrUndefined(o) {
			return o === null || o === undefined;
		};

		self.initialize = function (plotLocations, confidentialPlotLocations) {

			var plotFeatures = new Array();
			var plotFeaturesCorner = new Array();
			var confidentialPlotFeatures = new Array();
			//var confidentialPlotFeaturesCorner = new Array();

			var format = new ol.format.WKT();

			for (var i = 0; i < plotLocations.length; i++) {
				if (plotLocations[i].geometryWKT !== null && plotLocations[i].geometryWKT.trim() !== "") {
					var feature = format.readFeature(plotLocations[i].geometryWKT, {
						dataProjection: "EPSG:4326",
						featureProjection: "EPSG:3857"
					});

					feature.id = plotLocations[i].id;
					feature.name = plotLocations[i].plotName;
					feature.network = plotLocations[i].network;

					plotFeatures.push(feature);
				}
			}

			for (var j = 0; j < confidentialPlotLocations.length; j++) {
				if (confidentialPlotLocations[j].geometryWKT !== null && confidentialPlotLocations[j].geometryWKT.trim() !== "") {
					var confidentialFeature = format.readFeature(confidentialPlotLocations[j].geometryWKT, {
						dataProjection: "EPSG:4326",
						featureProjection: "EPSG:3857"
					});

					confidentialFeature.id = confidentialPlotLocations[j].id;
					confidentialFeature.name = confidentialPlotLocations[j].plotName;
					confidentialFeature.network = confidentialPlotLocations[j].network;

					confidentialPlotFeatures.push(confidentialFeature);
				}
			}

			var plotLayer = new ol.layer.Vector({
				title: "Plot data available here",
				visible: true,
				source: new ol.source.Vector({
					features: new ol.Collection(plotFeatures)
				}),
				style: styleFunction
				//style: new ol.style.Style({
				//	image: new ol.style.Circle({
				//		radius: 4,
				//		fill: new ol.style.Fill({
				//			color: "#cccccc"
				//		}),
				//		stroke: new ol.style.Stroke({
				//			color: "#33aa33",
				//			width: 1
				//		})
				//	})
				//})
			});

			var confidentialPlotLayer = new ol.layer.Vector({
				title: "Potentially available, curated by partner networks",
				visible: true,
				source: new ol.source.Vector({
					features: new ol.Collection(confidentialPlotFeatures)
				}),
				style: new ol.style.Style({
					image: new ol.style.Circle({
						radius: 4,
						fill: new ol.style.Fill({
							color: "#cccccc"
						}),
						stroke: new ol.style.Stroke({
							color: "#FFFFFF",
							width: 1
						})
					})
				})
			});

			//********************** Polygons (geometry_corner) ********************
			for (var i = 0; i < plotLocations.length; i++) {
				if (plotLocations[i].geometry_CornerWKT !== null && plotLocations[i].geometry_CornerWKT.trim() !== "") {
					var feature = format.readFeature(plotLocations[i].geometry_CornerWKT, {
						dataProjection: "EPSG:4326",
						featureProjection: "EPSG:3857"
					});

					feature.id = plotLocations[i].id;
					feature.name = plotLocations[i].plotName;
					feature.network = plotLocations[i].network;

					plotFeaturesCorner.push(feature);
				}
			}

			for (var j = 0; j < confidentialPlotLocations.length; j++) {
				if (confidentialPlotLocations[j].geometry_CornerWKT !== null && confidentialPlotLocations[j].geometry_CornerWKT.trim() !== "") {
					var confidentialFeature = format.readFeature(confidentialPlotLocations[j].geometry_CornerWKT, {
						dataProjection: "EPSG:4326",
						featureProjection: "EPSG:3857"
					});

					confidentialFeature.id = confidentialPlotLocations[j].id;
					confidentialFeature.name = confidentialPlotLocations[j].plotName;
					confidentialFeature.network = confidentialPlotLocations[j].network;

					plotFeaturesCorner.push(confidentialFeature);
				}
			}

			var plotCornerLayer = new ol.layer.Vector({
				title: "Display plots' shape (plots'corners)",
				visible: true,
				source: new ol.source.Vector({
					features: new ol.Collection(plotFeaturesCorner)
				}),
				style: styleFunction_polygons
				//style: new ol.style.Style({
				//	stroke: new ol.style.Stroke({
				//		color: 'blue',
				//		lineDash: [4],
				//		width: 3
				//	}),
				//	fill: new ol.style.Fill({
				//		color: 'rgba(0, 0, 255, 0.1)'
				//	})
				//})
			});

			//add to plotCornerLayer --> display plots' shape (plots'corners)
			//for (var j = 0; j < confidentialPlotLocations.length; j++) {
			//	if (confidentialPlotLocations[j].geometry_CornerWKT !== null && confidentialPlotLocations[j].geometry_CornerWKT.trim() !== "") {
			//		var confidentialFeature = format.readFeature(confidentialPlotLocations[j].geometry_CornerWKT, {
			//			dataProjection: "EPSG:4326",
			//			featureProjection: "EPSG:3857"
			//		});

			//		confidentialFeature.id = confidentialPlotLocations[j].id;
			//		confidentialFeature.name = confidentialPlotLocations[j].plotName;
			//		confidentialFeature.network = confidentialPlotLocations[j].network;

			//		confidentialPlotFeaturesCorner.push(confidentialFeature);
			//	}
			//}

			//var confidentialPlotCornerLayer = new ol.layer.Vector({
			//	title: "Confidential corner data available here",
			//	visible: true,
			//	source: new ol.source.Vector({
			//		features: new ol.Collection(confidentialFeature)
			//	}),
			//	style: new ol.style.Style({
			//		stroke: new ol.style.Stroke({
			//			color: 'green',
			//			lineDash: [4],
			//			width: 3
			//		}),
			//		fill: new ol.style.Fill({
			//			color: 'rgba(0, 0, 255, 0.1)'
			//		})
			//	})
			//});

			/***************** clusterd layers *****************/
			var cluster_distance = 20;

			var source_plotFeatures = new ol.source.Vector({
				features: new ol.Collection(plotFeatures)
			});

			var clusterSource_plotFeature = new ol.source.Cluster({
				distance: cluster_distance,
				source: source_plotFeatures
			});

			var clusters_plotFeature = new ol.layer.Vector({
				source: clusterSource_plotFeature,
				visible: true,
				style: styleFunction_clustered
			});

			var source_confidentialPlotFeatures = new ol.source.Vector({
				features: new ol.Collection(confidentialPlotFeatures)
			});

			var clusterSource_confidentialPlotFeatures = new ol.source.Cluster({
				distance: cluster_distance,
				source: source_confidentialPlotFeatures
			});

			var styleCache_confidentialPlotFeatures = {};
			var clusters_confidentialPlotFeatures = new ol.layer.Vector({
				source: clusterSource_confidentialPlotFeatures,
				visible: true,
				style: function style(feature) {
					var size = feature.get('features').length;
					var style = styleCache_confidentialPlotFeatures[size];
					if (!style) {
						style = new ol.style.Style({
							image: new ol.style.Circle({
								radius: 10,
								stroke: new ol.style.Stroke({
									color: '#fff'
								}),
								fill: new ol.style.Fill({
									color: '#ffffff'
								})
							}),
							text: new ol.style.Text({
								text: size.toString(),
								fill: new ol.style.Fill({
									color: '#000000'
								})
							})
						});
						styleCache_confidentialPlotFeatures[size] = style;
					}
					return style;
				}
			});

			/***************** end clusterd layers *****************/

			var map = new ol.Map({
				target: "map",
				layers: [new ol.layer.Group({
					'title': "Base maps",
					layers: [
					// OSM is not working atm.
					////new ol.layer.Tile({
					////    title: "OpenStreetMap",
					////    type: "base",
					////    visible: false,
					////    source: new ol.source.OSM()
					////}),
					new ol.layer.Tile({
						title: "Satellite",
						visible: true,
						type: "base",
						preload: Infinity,
						source: new ol.source.BingMaps({
							key: "Ar15IjogfL-tAKydaSWky9VA4om_p2J9jq2H0U2Ubp5muXDVPIBcuMXoScPlar0X",
							imagerySet: "Aerial",
							maxZoom: 19
						})
					}), new ol.layer.Tile({
						title: "Satellite with Labels",
						visible: false,
						type: "base",
						preload: Infinity,
						source: new ol.source.BingMaps({
							key: "Ar15IjogfL-tAKydaSWky9VA4om_p2J9jq2H0U2Ubp5muXDVPIBcuMXoScPlar0X",
							imagerySet: "AerialWithLabels",
							maxZoom: 19
						})
					}), new ol.layer.Tile({
						title: "Roadmap",
						visible: false,
						type: "base",
						preload: Infinity,
						source: new ol.source.BingMaps({
							key: "Ar15IjogfL-tAKydaSWky9VA4om_p2J9jq2H0U2Ubp5muXDVPIBcuMXoScPlar0X",
							imagerySet: "Road",
							maxZoom: 19
						})
					})]
				}), new ol.layer.Group({
					title: "Overlays",
					layers: [new ol.layer.Tile({
						title: "Hansen Tree Cover 2000",
						visible: false,
						source: new ol.source.XYZ({
							attributions: [attribution],
							url: "https://storage.googleapis.com/earthenginepartners-hansen/tiles/gfc2015/tree_alpha/{z}/{x}/{y}.png"
						})
					}),
					//new ol.layer.Tile({
					//	title: "Hansen loss",
					//	visible: false,
					//	source: new ol.source.XYZ({
					//		attributions: [attribution],
					//		url: "https://storage.googleapis.com/earthenginepartners-hansen/tiles/gfc2015/loss_alpha/{z}/{x}/{y}.png"
					//	})
					//}),
					//new ol.layer.Tile({
					//	title: "Hansen gain",
					//	visible: false,
					//	source: new ol.source.XYZ({
					//		attributions: [attribution],
					//		url: "https://storage.googleapis.com/earthenginepartners-hansen/tiles/gfc2015/gain_alpha/{z}/{x}/{y}.png"
					//	})
					//}),
					//new ol.layer.Tile({
					//	title: "IIASA hybrid biomass",
					//	visible: false,
					//	source: new ol.source.TileWMS(({
					//		url: "https://wms.geo-wiki.org/cgi-bin/biomasswms",
					//		params: { "LAYERS": "BmAbove_Ha", "TILED": true },
					//		serverType: "mapserver"
					//	}))
					//}),
					//new ol.layer.Tile({
					//	title: "Pan Boreal",
					//	visible: false,
					//	source: new ol.source.TileWMS(({
					//		url: "https://wms.geo-wiki.org/cgi-bin/biomasswms",
					//		params: { "LAYERS": "Boreal_ag_Turner", "TILED": true },
					//		serverType: "mapserver"
					//	}))
					//}),
					new ol.layer.Tile({
						title: "airborne LiDAR-based biomass maps",
						visible: false,
						source: new ol.source.TileWMS({
							url: "https://wms.geo-wiki.org/cgi-bin/biomasswms",
							params: { "LAYERS": "AfriSAR_Lope_AGB_50m,AfriSAR_Mabounie_AGB_50m,AfriSAR_Mondah_AGB_50m,AfriSAR_Rabi_AGB_50m,TropiSAR_Arbocel_AGB_50m,TropiSAR_Nouragues_AGB_50m,TropiSAR_Paracou_AGB_50m", "TILED": true },
							serverType: "mapserver",
							attributions: [new ol.Attribution({ html: '<a href="https://doi.org/10.1109/JSTARS.2018.2851606" target="_blank" style="font-size: 12px; display: block; text-align: left; font-weight: bolder">LiDAR map reference</a><br/>' })]
						})
					}), new ol.layer.Tile({
						title: "Tropics by WUR",
						visible: false,
						source: new ol.source.TileWMS({
							url: "https://wms.geo-wiki.org/cgi-bin/biomasswms",
							params: { "LAYERS": "Avitabile_AGB_Map", "TILED": true },
							serverType: "mapserver"
						})
					}), new ol.layer.Tile({
						title: "GlobBiomass",
						visible: false,
						source: new ol.source.TileWMS({
							url: "https://wms.geo-wiki.org/cgi-bin/biomasswms",
							params: { "LAYERS": "GlobBiomass", "TILED": true },
							serverType: "mapserver"
						})
					})]
				}), new ol.layer.Group({
					title: "Plots",
					layers: [
					////new ol.layer.Vector({
					////	title: "Potentially available, curated by partner networks",
					////	visible: true,
					////	source: new ol.source.Vector({
					////		url: "/data/PlotsFeb2017.kml",
					////		format: new ol.format.KML({
					////			extractStyles: false
					////		})
					////	}),
					////	style: new ol.style.Style({
					////		image: new ol.style.Circle({
					////			radius: 4,
					////			fill: new ol.style.Fill({
					////				color: "#cccccc"
					////			}),
					////			stroke: new ol.style.Stroke({
					////				color: "#3333aa",
					////				width: 1
					////			})
					////		})
					////	})
					////}),
					confidentialPlotLayer, plotLayer, plotCornerLayer]
				}),
				//confidentialPlotCornerLayer
				new ol.layer.Group({
					title: "clusters",
					layers: [clusters_plotFeature, clusters_confidentialPlotFeatures]
				})],
				view: new ol.View({
					center: ol.proj.transform([20, 0], "EPSG:4326", "EPSG:3857"),
					zoom: 3
				})
			});

			/***************** clusterd layers visibility *****************/
			map.getView().on('propertychange', function (e) {
				switch (e.key) {
					case 'resolution':
						var zoom = parseInt(map.getView().getZoom());
						if (zoom >= 15) {
							clusters_plotFeature.setVisible(false);
							clusters_confidentialPlotFeatures.setVisible(false);
						} else if (zoom < 15) {
							clusters_plotFeature.setVisible(true);
							clusters_confidentialPlotFeatures.setVisible(true);
						}
						break;
				}
			});
			/***************** end clusterd layers visibility *****************/
			var layerSwitcher = new ol.control.LayerSwitcher({
				tipLabel: "Legend" // Optional label for button
			});

			map.addControl(layerSwitcher);

			var info = $("#info");
			info.tooltip({
				animation: false,
				trigger: "manual"
			});

			var displayFeatureInfo = function displayFeatureInfo(pixel, data) {
				info.css({
					left: pixel[0] + 15 + "px",
					top: pixel[1] /* - 5 */ + "px"
				});
				var feature = map.forEachFeatureAtPixel(pixel, function (feature) {
					return feature;
				});

				if (feature) {
					var name = feature.name;
					if (IsNullOrUndefined(name)) name = ""; //name = feature.get("name");

					info.tooltip("hide").attr("data-original-title", name).tooltip("fixTitle").tooltip("show");
				} else if (!IsNullOrUndefined(data)) {
					var details = "";
					for (var key in data) {
						if (data.hasOwnProperty(key)) {
							details += key + ": " + Math.round(data[key]) + "\n";
						}
					}

					info.tooltip("hide").attr("data-original-title", details).tooltip("fixTitle").tooltip("show");
				} else {
					info.tooltip("hide");
				}
			};

			map.on("pointermove", function (evt) {
				if (evt.dragging) {
					info.tooltip("hide");
					return;
				}

				displayFeatureInfo(map.getEventPixel(evt.originalEvent));
			});

			var element = document.getElementById("popup");

			var popup = new ol.Overlay({
				element: element,
				positioning: "bottom-center",
				stopEvent: false
			});
			map.addOverlay(popup);

			map.on("click", function (evt) {
				self.viewModel.title("");
				self.viewModel.siteDetails("");
				self.viewModel.plotDetails("");
				self.viewModel.photoUrl("");
				self.viewModel.link("");
				self.viewModel.loading(false);

				var feature = map.forEachFeatureAtPixel(evt.pixel, function (feature, layer) {
					return feature;
				});

				if (!IsNullOrUndefined(feature) && !IsNullOrUndefined(feature.id)) {
					var id = feature.id;
					self.viewModel.loading(true);
					setTimeout(function () {
						$.getJSON("/plot/" + id, function (data) {
							if (!IsNullOrUndefined(data)) {

								self.viewModel.loading(false);

								self.viewModel.title(data.siteName + " (" + data.plotName + ")");

								var siteDetails = "";
								if (!IsNullOrUndefined(data.country)) {
									siteDetails += "<p>" + data.country + "</p>";
								}
								if (!IsNullOrUndefined(data.network) && data.network.trim() !== "") {
									siteDetails += "<p><b>Network:</b> " + data.network + "</p>";
								}
								if (!IsNullOrUndefined(data.institutions) && data.institutions.trim() !== "") {
									siteDetails += "<p><b>Institutions:</b> " + data.institutions + "</p>";
								}
								if (data.network !== null && data.url.trim() !== "") {
									siteDetails += "<p><b>Link:</b> <a href=\"" + data.url + "\" target=\"_blank\">" + data.url + "</a></p>";
								}
								//PIs wrong order --> read from table principal_investigator
								//if (!IsNullOrUndefined(data.pIs) && data.pIs.trim() !== "") {
								//	siteDetails += "<p><b>PIs:</b> " + data.pIs + "</p>";
								//}
								//PIs correct order --> read from table feature column pi_team
								if (!IsNullOrUndefined(data.pIs_text) && data.pIs_text.trim() !== "") {
									siteDetails += "<p><b>PIs:</b> " + data.pIs_text + "</p>";
								}

								if (!IsNullOrUndefined(data.area)) {
									siteDetails += "<p><b>Sub-plot area:</b> " + data.area;
									if (!IsNullOrUndefined(data.area_sum)) {
										siteDetails += "; <b>Plot area:</b> " + data.area_sum;
									}
									siteDetails += "</p>";
								}

								if (!IsNullOrUndefined(data.altitude) || !IsNullOrUndefined(data.slope)) {
									siteDetails += "<p>";
									if (!IsNullOrUndefined(data.altitude)) {
										siteDetails += "<b>Altitude:</b> " + data.altitude + "&nbsp;&nbsp;&nbsp;";
									}
									if (!IsNullOrUndefined(data.slope)) {
										siteDetails += "<b>Slope:</b> " + data.slope;
									}
									siteDetails += "</p>";
								}

								if (!IsNullOrUndefined(data.biomassProcessingProtocol) && data.biomassProcessingProtocol.trim() !== "") {
									siteDetails += "<p><b>Biomass Processing Protocol:</b> <a href=\"/Docs/biomass_processing_protocol/" + data.biomassProcessingProtocol + "\" target=\"_blank\">" + data.biomassProcessingProtocol + "</a></p>";
								}
								if (!IsNullOrUndefined(data.establishedDate)) {
									siteDetails += "<p><b>Established:</b> " + data.establishedDate + "</p>";
								}
								self.viewModel.siteDetails(siteDetails);

								var plotDetails = "";
								if (!IsNullOrUndefined(data.censuses) && data.censuses.length > 0) {
									var census = data.censuses[0];

									if (!IsNullOrUndefined(census.censusDate)) {
										plotDetails += "<p><b>Census:</b> " + census.censusDate + "</p>";
									}
									if (!IsNullOrUndefined(census.measurements) && census.measurements.length > 0) {
										plotDetails += "<p><b>Measurements:</b> <br/>";

										for (var i = 0; i < census.measurements.length; i++) {
											plotDetails += census.measurements[i].measurement;
											if (census.measurements[i].additionalMeasurements !== null && census.measurements[i].additionalMeasurements.length > 0) {
												plotDetails += " <i class=\"fa fa-info-circle\" aria-hidden=\"true\" title=\"";
												plotDetails += census.measurements[i].additionalMeasurements.join("\n");

												plotDetails += "\"></i>";
											}

											plotDetails += "<br/>";
										}

										plotDetails += "</p>";
									}

									if (!IsNullOrUndefined(census.taxonomicIdentifications) && census.taxonomicIdentifications.length > 0) {
										plotDetails += "<p><b>Taxonomic Identifications:</b> <br/>";

										for (var ti = 0; ti < census.taxonomicIdentifications.length; ti++) {
											plotDetails += census.taxonomicIdentifications[ti] + "<br/>";
										}

										plotDetails += "</p>";
									}
								}

								self.viewModel.plotDetails(plotDetails);

								if (data.photos.length > 0) {
									self.viewModel.photoUrl("https://forest-observation-system.net/Images/sites/" + data.photos[0]);
								}

								if (!IsNullOrUndefined(data.reference) && data.reference.trim() !== "") {
									self.viewModel.link("<p><b>Reference:</b> " + data.reference + "</p>");
								}
							} else {
								self.viewModel.title(feature.get("name"));
								self.viewModel.siteDetails(feature.get("description"));
							}
						});
					}, 800);
				} else if (map.getView().getZoom() > 12) {
					var location = ol.proj.transform(evt.coordinate, 'EPSG:3857', 'EPSG:4326');
					var lon = location[0]; //11.610106;
					var lat = location[1]; //-0.221219;

					// Call the Geo-Wiki API
					api.callJson("Landcover", "getIFBNLidarValues", { latitude: lat, longitude: lon }, function (data) {

						if (!$.isEmptyObject(data)) {
							displayFeatureInfo(map.getEventPixel(evt.originalEvent), data);
						}
					});
				}
			});

			$("#downloadButton").on("click", function () {
				var accept_terms_conditions = $("#form_TermsConditions input:checked[type='checkbox']").val();
				if (accept_terms_conditions !== null && typeof accept_terms_conditions !== 'undefined' && accept_terms_conditions.toString() === "true") {

					var intendedUse = $("#form_IntendedUse [type='text']").val();
					if (intendedUse.trim() != "") {

						var country = $("#countrySelect").val();
						var downloadtype = $("#form_DownloadType input:checked[type='radio']").val();

						if (downloadtype === "csv") {
							window.location = "/DownloadCSV/" + country + "/" + intendedUse;
						} else {
							window.location = "/download/" + country + "/" + intendedUse;
						}
					} else {
						alert("Please enter intended use before downloading");
					}
				} else {
					alert("Please accept terms and conditions before downloading");
				}
			});

			$("#downloadButtonBBox").on("click", function () {
				var accept_terms_conditions = $("#form_TermsConditions input:checked[type='checkbox']").val();
				if (accept_terms_conditions !== null && typeof accept_terms_conditions !== 'undefined' && accept_terms_conditions.toString() === "true") {

					var intendedUse = $("#form_IntendedUse [type='text']").val();
					if (intendedUse.trim() !== "") {

						var downloadtype = $("#form_DownloadType input:checked[type='radio']").val();
						var extent = map.getView().calculateExtent(map.getSize());
						var bbox = ol.proj.transformExtent(extent, 'EPSG:3857', 'EPSG:4326');

						//var bbox = "bbox=-180,-90,180,90";
						//var bbox = "-180,-90,180,90";

						if (downloadtype === "csv") {
							window.location = "/DownloadBBoxCSV/" + bbox + "/" + intendedUse;
							//alert("test");
							//window.location = "/DownloadBBoxCSV_test/" + bbox + "/" + intendedUse;
						} else {
								window.location = "/DownloadBBox/" + bbox + "/" + intendedUse;
							}
					} else {
						alert("Please enter intended use before downloading");
					}
				} else {
					alert("Please accept terms and conditions before downloading");
				}
			});

			ko.applyBindings(self.viewModel);
		};

		return self;
	})();
})(window);

