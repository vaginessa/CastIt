import 'dart:async';
import 'dart:io';

import 'package:flutter/services.dart';

const _methodChannelName = 'com.github.wolfteam.castit';
const _methodChannel = MethodChannel(_methodChannelName);

/// Static class that provides AppCenter APIs
class AppCenter {
  /// Start appcenter functionalities
  static Future<void> startAsync({
    required String appSecretAndroid,
    required String appSecretIOS,
    bool enableAnalytics = true,
    bool enableCrashes = true,
    bool enableDistribute = false,
    bool usePrivateDistributeTrack = false,
  }) async {
    if (Platform.isWindows) {
      return;
    }
    String appsecret;
    if (Platform.isAndroid) {
      appsecret = appSecretAndroid;
    } else if (Platform.isIOS) {
      appsecret = appSecretIOS;
    } else {
      throw UnsupportedError('Current platform is not supported.');
    }

    if (appsecret.isEmpty) {
      return;
    }

    await configureAnalyticsAsync(enabled: enableAnalytics);
    await configureCrashesAsync(enabled: enableCrashes);

    await _methodChannel.invokeMethod('start', <String, dynamic>{
      'secret': appsecret.trim(),
      'usePrivateTrack': usePrivateDistributeTrack,
    });
  }

  /// Track events
  static Future<void> trackEventAsync(String name, [Map<String, String>? properties]) async {
    if (Platform.isWindows) {
      return;
    }
    await _methodChannel.invokeMethod('trackEvent', <String, dynamic>{
      'name': name,
      'properties': properties ?? <String, String>{},
    });
  }

  /// Check whether analytics is enalbed
  static Future<bool> isAnalyticsEnabledAsync() async {
    if (Platform.isWindows) {
      return false;
    }
    final enabled = await _methodChannel.invokeMethod<bool?>('isAnalyticsEnabled');
    return enabled ?? false;
  }

  /// Get appcenter installation id
  static Future<String> getInstallIdAsync() async {
    return _methodChannel.invokeMethod('getInstallId').then((r) => r as String);
  }

  /// Enable or disable analytics
  static Future configureAnalyticsAsync({required bool enabled}) async {
    if (Platform.isWindows) {
      return;
    }
    await _methodChannel.invokeMethod('configureAnalytics', enabled);
  }

  /// Check whether crashes is enabled
  static Future<bool> isCrashesEnabledAsync() async {
    if (Platform.isWindows) {
      return false;
    }
    final enabled = await _methodChannel.invokeMethod<bool?>('isCrashesEnabled');
    return enabled ?? false;
  }

  /// Enable or disable appcenter crash reports
  static Future configureCrashesAsync({required bool enabled}) async {
    if (Platform.isWindows) {
      return;
    }
    await _methodChannel.invokeMethod('configureCrashes', enabled);
  }
}
