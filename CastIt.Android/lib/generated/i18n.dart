import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
// ignore_for_file: non_constant_identifier_names
// ignore_for_file: camel_case_types
// ignore_for_file: prefer_single_quotes
// ignore_for_file: unnecessary_brace_in_string_interps

//WARNING: This file is automatically generated. DO NOT EDIT, all your changes would be lost.

typedef LocaleChangeCallback = void Function(Locale locale);

class I18n implements WidgetsLocalizations {
  const I18n();
  static Locale _locale;
  static bool _shouldReload = false;

  static set locale(Locale newLocale) {
    _shouldReload = true;
    I18n._locale = newLocale;
  }

  static const GeneratedLocalizationsDelegate delegate = GeneratedLocalizationsDelegate();

  /// function to be invoked when changing the language
  static LocaleChangeCallback onLocaleChanged;

  static I18n of(BuildContext context) =>
    Localizations.of<I18n>(context, WidgetsLocalizations);

  @override
  TextDirection get textDirection => TextDirection.ltr;

  /// "CastIt"
  String get appName => "CastIt";
  /// "Dark"
  String get dark => "Dark";
  /// "Light"
  String get light => "Light";
  /// "English"
  String get english => "English";
  /// "Spanish"
  String get spanish => "Spanish";
  /// "Settings"
  String get settings => "Settings";
  /// "Theme"
  String get theme => "Theme";
  /// "Version: ${version}"
  String appVersion(String version) => "Version: ${version}";
  /// "Choose base app color"
  String get chooseBaseAppColor => "Choose base app color";
  /// "Accent Color"
  String get accentColor => "Accent Color";
  /// "Choose an accent color"
  String get chooseAccentColor => "Choose an accent color";
  /// "Language"
  String get language => "Language";
  /// "Choose a language"
  String get chooseLanguage => "Choose a language";
  /// "About"
  String get about => "About";
  /// "A remote control for the desktop app"
  String get aboutSummary => "A remote control for the desktop app";
  /// "Donations"
  String get donations => "Donations";
  /// "Support"
  String get support => "Support";
  /// "App information"
  String get appInfo => "App information";
  /// "I hope you are enjoying using this app, if you would like to buy me a coffee/beer, just send me an email."
  String get donationsMsg => "I hope you are enjoying using this app, if you would like to buy me a coffee/beer, just send me an email.";
  /// "I made this app in my free time and it is also open source. If you would like to help me, report an issue, have an idea, want a feature to be implemented, etc please open an issue here:"
  String get donationSupport => "I made this app in my free time and it is also open source. If you would like to help me, report an issue, have an idea, want a feature to be implemented, etc please open an issue here:";
  /// "Player Settings"
  String get playerSettings => "Player Settings";
  /// "Change app behaviour"
  String get changeAppBehaviour => "Change app behaviour";
  /// "PlayLists"
  String get playlists => "PlayLists";
  /// "Playlist"
  String get playlist => "Playlist";
  /// "Play files from the start"
  String get playFromTheStart => "Play files from the start";
  /// "Play next file automatically"
  String get playNextFileAutomatically => "Play next file automatically";
  /// "Force video transcode"
  String get forceVideoTranscode => "Force video transcode";
  /// "Force audio transcode"
  String get forceAudioTranscode => "Force audio transcode";
  /// "Enable hardware acceleration"
  String get enableHwAccel => "Enable hardware acceleration";
  /// "Video scale"
  String get videoScale => "Video scale";
  /// "Playing"
  String get playing => "Playing";
  /// "Full HD (1080p)"
  String get fullHd => "Full HD (1080p)";
  /// "HD (720p)"
  String get hd => "HD (720p)";
  /// "Original"
  String get original => "Original";
  /// "Something went wrong!"
  String get somethingWentWrong => "Something went wrong!";
  /// "Please try again later"
  String get pleaseTryAgainLater => "Please try again later";
  /// "Make sure you are connected to the same wifi network of the desktop app and that the desktop app is opened"
  String get makeSureYouAreConnected => "Make sure you are connected to the same wifi network of the desktop app and that the desktop app is opened";
  /// "Url"
  String get url => "Url";
  /// "Invalid Url"
  String get invalidUrl => "Invalid Url";
  /// "Cancel"
  String get cancel => "Cancel";
  /// "Ok"
  String get ok => "Ok";
  /// "No connection to the desktop app"
  String get noConnectionToDesktopApp => "No connection to the desktop app";
  /// "Verify that the url matches the one in the desktop app"
  String get verifyCastItUrl => "Verify that the url matches the one in the desktop app";
}

class _I18n_en_US extends I18n {
  const _I18n_en_US();

  @override
  TextDirection get textDirection => TextDirection.ltr;
}

class _I18n_es_VE extends I18n {
  const _I18n_es_VE();

  /// "CastIt"
  @override
  String get appName => "CastIt";
  /// "Oscuro"
  @override
  String get dark => "Oscuro";
  /// "Ligero"
  @override
  String get light => "Ligero";
  /// "Inglés"
  @override
  String get english => "Inglés";
  /// "Español"
  @override
  String get spanish => "Español";
  /// "Ajustes"
  @override
  String get settings => "Ajustes";
  /// "Tema"
  @override
  String get theme => "Tema";
  /// "Versión: ${version}"
  @override
  String appVersion(String version) => "Versión: ${version}";
  /// "Escoge un color base para la app"
  @override
  String get chooseBaseAppColor => "Escoge un color base para la app";
  /// "Color de acento"
  @override
  String get accentColor => "Color de acento";
  /// "Escoge un color de acento"
  @override
  String get chooseAccentColor => "Escoge un color de acento";
  /// "Idioma"
  @override
  String get language => "Idioma";
  /// "Escoge un idioma"
  @override
  String get chooseLanguage => "Escoge un idioma";
  /// "Acerca de"
  @override
  String get about => "Acerca de";
  /// "Control remoto para la aplicación de escritorio."
  @override
  String get aboutSummary => "Control remoto para la aplicación de escritorio.";
  /// "Donaciones"
  @override
  String get donations => "Donaciones";
  /// "Soporte"
  @override
  String get support => "Soporte";
  /// "Información de la aplicación"
  @override
  String get appInfo => "Información de la aplicación";
  /// "Espero que disfrutes al usar esta aplicación, si te gustaría comprarme un cafe o una cerveza, envíame un email."
  @override
  String get donationsMsg => "Espero que disfrutes al usar esta aplicación, si te gustaría comprarme un cafe o una cerveza, envíame un email.";
  /// "Hice esta aplicación en mi tiempo libre y también es de código abierto. Si desea ayudarme, informar un problema, tienes una idea, deseas que se implemente una función, etc., crea un issue aquí:"
  @override
  String get donationSupport => "Hice esta aplicación en mi tiempo libre y también es de código abierto. Si desea ayudarme, informar un problema, tienes una idea, deseas que se implemente una función, etc., crea un issue aquí:";
  /// "Player Settings"
  @override
  String get playerSettings => "Player Settings";
  /// "Cambiar el comportamiento de la aplicación"
  @override
  String get changeAppBehaviour => "Cambiar el comportamiento de la aplicación";
  /// "Listas de reproducción"
  @override
  String get playlists => "Listas de reproducción";
  /// "Lista de reproducción"
  @override
  String get playlist => "Lista de reproducción";
  /// "Reproducir archivos desde el inicio"
  @override
  String get playFromTheStart => "Reproducir archivos desde el inicio";
  /// "Reproducir el siguiente archivo automáticamente"
  @override
  String get playNextFileAutomatically => "Reproducir el siguiente archivo automáticamente";
  /// "Forzar la transcodificación del video"
  @override
  String get forceVideoTranscode => "Forzar la transcodificación del video";
  /// "Forzar la transcodificación del audio"
  @override
  String get forceAudioTranscode => "Forzar la transcodificación del audio";
  /// "Habilitar la aceleración por hardware"
  @override
  String get enableHwAccel => "Habilitar la aceleración por hardware";
  /// "Escala de video"
  @override
  String get videoScale => "Escala de video";
  /// "Reproduciendo"
  @override
  String get playing => "Reproduciendo";
  /// "Full HD (1080p)"
  @override
  String get fullHd => "Full HD (1080p)";
  /// "HD (720p)"
  @override
  String get hd => "HD (720p)";
  /// "Original"
  @override
  String get original => "Original";
  /// "Algo salió mal!"
  @override
  String get somethingWentWrong => "Algo salió mal!";
  /// "Porfavor intente mas tarde"
  @override
  String get pleaseTryAgainLater => "Porfavor intente mas tarde";
  /// "Asegurate de que estás conectado a la misma red wifi que la aplicación de escritorio y de que la misma esta abierta"
  @override
  String get makeSureYouAreConnected => "Asegurate de que estás conectado a la misma red wifi que la aplicación de escritorio y de que la misma esta abierta";
  /// "Url"
  @override
  String get url => "Url";
  /// "Url no válida"
  @override
  String get invalidUrl => "Url no válida";
  /// "Cancelar"
  @override
  String get cancel => "Cancelar";
  /// "Ok"
  @override
  String get ok => "Ok";
  /// "No hay conexión con la aplicación de escritorio"
  @override
  String get noConnectionToDesktopApp => "No hay conexión con la aplicación de escritorio";
  /// "Verifica que la url concuerde con la de la aplicación de escritorio"
  @override
  String get verifyCastItUrl => "Verifica que la url concuerde con la de la aplicación de escritorio";

  @override
  TextDirection get textDirection => TextDirection.ltr;
}

class GeneratedLocalizationsDelegate extends LocalizationsDelegate<WidgetsLocalizations> {
  const GeneratedLocalizationsDelegate();
  List<Locale> get supportedLocales {
    return const <Locale>[
      Locale("en", "US"),
      Locale("es", "VE")
    ];
  }

  LocaleResolutionCallback resolution({Locale fallback}) {
    return (Locale locale, Iterable<Locale> supported) {
      if (isSupported(locale)) {
        return locale;
      }
      final Locale fallbackLocale = fallback ?? supported.first;
      return fallbackLocale;
    };
  }

  @override
  Future<WidgetsLocalizations> load(Locale locale) {
    I18n._locale ??= locale;
    I18n._shouldReload = false;
    final String lang = I18n._locale != null ? I18n._locale.toString() : "";
    final String languageCode = I18n._locale != null ? I18n._locale.languageCode : "";
    if ("en_US" == lang) {
      return SynchronousFuture<WidgetsLocalizations>(const _I18n_en_US());
    }
    else if ("es_VE" == lang) {
      return SynchronousFuture<WidgetsLocalizations>(const _I18n_es_VE());
    }
    else if ("en" == languageCode) {
      return SynchronousFuture<WidgetsLocalizations>(const _I18n_en_US());
    }
    else if ("es" == languageCode) {
      return SynchronousFuture<WidgetsLocalizations>(const _I18n_es_VE());
    }

    return SynchronousFuture<WidgetsLocalizations>(const I18n());
  }

  @override
  bool isSupported(Locale locale) {
    for (var i = 0; i < supportedLocales.length && locale != null; i++) {
      final l = supportedLocales[i];
      if (l.languageCode == locale.languageCode) {
        return true;
      }
    }
    return false;
  }

  @override
  bool shouldReload(GeneratedLocalizationsDelegate old) => I18n._shouldReload;
}